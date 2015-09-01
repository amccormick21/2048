using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System;
using System.Windows.Media.Animation;
using _2048Game.GamePlay;
using _2048Game.Board;
using System.IO;

namespace _2048Game.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public delegate void MoveBlocksEventHandler(DragDirection direction);
        public event MoveBlocksEventHandler MoveBlocks;

        Label LoseLabel;
        public Game game;
        List<NumberLabel> LabelsOnBoard;
        MoveMadeEventArgs lastMoveEvents;
        double SpeedOfAnimations;

        bool slideFinished, pulseFinished, growFinished;

        private Storyboard slideStoryBoard;
        private Storyboard pulseStoryBoard;
        private Storyboard growStoryBoard;

        public MainWindow()
        {
            InitializeComponent();
            InitialiseLabels();
            LoadDataFromFile();
            slideFinished = pulseFinished = growFinished = false;
            game = new Game(this);
            Restart.MouseDown += Restart_MouseDown;
            Undo.MouseDown += Undo_MouseDown;
            AI.Click += AI_Click;
            AISpeed.ValueChanged += AISpeed_ValueChanged;
            FixedRandom.Checked += FixedRandom_Checked;
            AISpeed.Value = 9.2;
            game.GameOver += Game_GameOver;
            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
        }

        private void LoadDataFromFile()
        {
            StreamReader sr = new StreamReader("Stats.txt");
            App.HighScore = Convert.ToInt32(sr.ReadLine());
            
            HighScoreBox.Text = Convert.ToString(App.HighScore);
            sr.Dispose();
        }

        void FixedRandom_Checked(object sender, RoutedEventArgs e)
        {
            if (FixedRandom.IsChecked.HasValue)
            {
                App.FixedRandom = (bool)FixedRandom.IsChecked.Value;
            }
        }

        void AISpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SpeedOfAnimations = e.NewValue;
        }

        void AI_Click(object sender, RoutedEventArgs e)
        {
            //Setting AIPlay fires an event which starts the AI in the game
            if (AI.IsChecked.HasValue)
            {
                if ((bool)AI.IsChecked)
                {
                    game.AIPlay = true;
                }
                else
                {
                    game.AIPlay = false;
                }
            }
            else
            {
                game.AIPlay = false;
            }
        }

        private void InitialiseLabels()
        {
            LabelsOnBoard = new List<NumberLabel>();
        }

        void Undo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            game.UndoLastMove();
        }

        void Game_GameOver(GameOverEventArgs e)
        {
            ShowEndOfGame();
        }

        public void Restart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                OnRestartPressed();
        }

        public void OnRestartPressed()
        {
            if (App.GameRunning)
                game.OnGameOver();
            RestartGame();
        }

        private void RestartGame()
        {
            ClearGrid();
            HideEndOfGame();
            this.game.GameOver -= Game_GameOver;
            this.game = new Game(this);
            this.game.GameOver += Game_GameOver;
        }

        private void ClearGrid()
        {
            Board.Children.Clear();
            LabelsOnBoard.Clear();
        }

        public void AddLabel(NumberLabel Label, int row, int column)
        {
            NumberLabel LabelCopy = new NumberLabel(Label);
            LabelsOnBoard.Add(LabelCopy);
            Board.Children.Add(LabelCopy);
            Canvas.SetLeft(LabelCopy, GetGridPosition(column, true));
            Canvas.SetTop(LabelCopy, GetGridPosition(row, false));
        }

        public void SetScore(int Score)
        {
            ScoreBox.Text = Convert.ToString(Score);
        }

        void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left: MoveBlocks(DragDirection.Left); break;
                case Key.Up: MoveBlocks(DragDirection.Up); break;
                case Key.Right: MoveBlocks(DragDirection.Right); break;
                case Key.Down: MoveBlocks(DragDirection.Down); break;
                default: break;
            }
        }

        void ShowEndOfGame()
        {
            //ClearGrid();
            LoseLabel = new Label();
            LoseLabel.Content = "You Lose!";
            this.Board.Children.Add(LoseLabel);
            Canvas.SetLeft(LoseLabel, 0);
            Canvas.SetRight(LoseLabel, 0);
            Canvas.SetTop(LoseLabel, 0);
            Canvas.SetBottom(LoseLabel, 0);
        }
        void HideEndOfGame()
        {
            this.Board.Children.Remove(LoseLabel);
            InitialiseLabels();
        }

        internal void ShowGrid(PlayingGrid playingGrid)
        {
            ClearGrid();
            LabelsOnBoard.Clear();
            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    if (playingGrid[row, column] != 0)
                    {
                        AddLabel(new NumberLabel(playingGrid.Cells[row, column], Board.Width / 4, Board.Height / 4), row, column);
                    }
                }
            }
        }

        internal void UpdateLabelsOnBoard(MoveMadeEventArgs e)
        {
            //Save the event args.
            lastMoveEvents = e;
            //Set all animations to not finished.
            slideFinished = pulseFinished = growFinished = false;

            //Return each cell to its rightful position with the animation.
            SetupMotionAnimations(e.Moves);
            if (e.Moves.Count != 0)
                RunMotionAnimations();
            else
                slideFinished = true;
        }

        void FinishAnimations()
        {
            //Arrange the array so it represents what is actually seen
            ArrangeGrid(lastMoveEvents.Moves);
            DeHighlightCells();

            //Pulse the cells that are going to have their values updated
            SetupPulseAnimations(lastMoveEvents.ValuesChanged);
            if (lastMoveEvents.ValuesChanged.Count != 0)
                RunPulseAnimations();
            else
                pulseFinished = true;
            //Update the values
            UpdateValues(lastMoveEvents.ValuesChanged);

            //Setup the cell to be grown in
            //Update its value so that when it grows the cell is coloured
            UpdateCellValue(lastMoveEvents.NewCell);
            SetupGrowAnimations(lastMoveEvents.NewCell);
            //Run the animation to bring in the new cell
            RunGrowAnimations();
        }

        private void DeHighlightCells()
        {
            foreach (NumberLabel Label in LabelsOnBoard)
            {
                Label.GoWhite();
            }
        }

        internal void UpdateCellValue(Cell cell)
        {
            NumberLabel LabelToAdd = new NumberLabel(cell, Board.Width / 4, Board.Height / 4);
            AddLabel(LabelToAdd, cell.Row, cell.Column);
        }

        private void UpdateValues(List<PositionOnBoard> ValuesChanged)
        {
            foreach (PositionOnBoard position in ValuesChanged)
            {
                GetLabel(position.Row, position.Column).Number *= 2;
            }
        }

        private void SetupGrowAnimations(Cell cell)
        {
            double duration = 180 - SpeedOfAnimations * 14;

            var GrowSize = new ThicknessAnimation()
            {
                From = new Thickness(48),
                To = new Thickness(6),
                Duration = new Duration(TimeSpan.FromMilliseconds(duration)),
                AutoReverse = false
            };
            Storyboard.SetTargetProperty(GrowSize, new PropertyPath(DockPanel.MarginProperty));
            Storyboard.SetTarget(GrowSize, GetLabel(cell.Row, cell.Column).BackgroundColourPanel);
            growStoryBoard = new Storyboard();
            growStoryBoard.Children.Add(GrowSize);
        }

        private void RunGrowAnimations()
        {
            growStoryBoard.Completed += growStoryBoard_Completed;
            growStoryBoard.Begin();
        }

        void growStoryBoard_Completed(object sender, EventArgs e)
        {
            growFinished = true;
            if (pulseFinished && slideFinished && AnimationsComplete != null)
                AnimationsComplete(this, new EventArgs());
        }

        private void SetupPulseAnimations(List<PositionOnBoard> ValuesChanged)
        {
            //Pulse
            double duration = 200 - SpeedOfAnimations * 16;

            pulseStoryBoard = new Storyboard();
            ThicknessAnimation cellPulse;

            foreach (PositionOnBoard position in ValuesChanged)
            {
                cellPulse = new ThicknessAnimation();
                cellPulse.From = new Thickness(6);
                cellPulse.To = new Thickness(0);
                cellPulse.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
                cellPulse.AutoReverse = true;

                pulseStoryBoard.Children.Add(cellPulse);
                Storyboard.SetTargetProperty(cellPulse, new PropertyPath(DockPanel.MarginProperty));
                Storyboard.SetTarget(cellPulse, GetLabel(position.Row, position.Column).BackgroundColourPanel);
            }
        }

        private void RunPulseAnimations()
        {
            pulseStoryBoard.Completed += pulseStoryBoard_Completed;
            pulseStoryBoard.Begin();
        }

        void pulseStoryBoard_Completed(object sender, EventArgs e)
        {
            pulseFinished = true;
            if (growFinished && slideFinished && AnimationsComplete != null)
                AnimationsComplete(this, new EventArgs());
        }

        private void SetupMotionAnimations(List<Move> Moves)
        {
            double duration = 1000 - SpeedOfAnimations * 99;
            slideStoryBoard = new Storyboard();
            DoubleAnimation slide;
            foreach (Move move in Moves)
            {
                if (move.SourceColumn == move.DestinationColumn) //Moving vertically
                {
                    slide = new DoubleAnimation()
                    {
                        From = GetGridPosition(move.SourceRow, true),
                        To = GetGridPosition(move.DestinationRow, true)
                    };
                    Storyboard.SetTargetProperty(slide, new PropertyPath(Canvas.TopProperty));
                }
                else
                {
                    slide = new DoubleAnimation()
                    {
                        From = GetGridPosition(move.SourceColumn, false),
                        To = GetGridPosition(move.DestinationColumn, false)
                    };
                    Storyboard.SetTargetProperty(slide, new PropertyPath(Canvas.LeftProperty));
                }
                slide.Duration = new Duration(TimeSpan.FromMilliseconds(duration));
                slide.AutoReverse = false;
                NumberLabel LabelToAnimate = GetLabel(move.SourceRow, move.SourceColumn);
                if (LabelToAnimate != null)
                {
                    Storyboard.SetTarget(slide, LabelToAnimate);
                    slideStoryBoard.Children.Add(slide);
                }
            }
        }

        private void RunMotionAnimations()
        {
            slideStoryBoard.Completed += slideStoryBoard_Completed;
            slideStoryBoard.Begin();
        }

        void slideStoryBoard_Completed(object sender, EventArgs e)
        {
            slideFinished = true;
            FinishAnimations();
            if (growFinished && pulseFinished && AnimationsComplete != null)
                AnimationsComplete(this, new EventArgs());
        }

        private void ArrangeGrid(List<Move> MovesMade)
        {         
            foreach (Move move in MovesMade)
            {
                Board.Children.Remove(GetLabel(move.DestinationRow, move.DestinationColumn));
                LabelsOnBoard.Remove(GetLabel(move.DestinationRow, move.DestinationColumn));

                NumberLabel LabelToUpdate = GetLabel(move.SourceRow, move.SourceColumn);
                LabelToUpdate.Row = move.DestinationRow;
                LabelToUpdate.Column = move.DestinationColumn;
            }
        }

        double GetGridPosition(int index, bool isLeftPosition)
        {
            return index * ((isLeftPosition ? Board.Width : Board.Height)/4);
        }

        NumberLabel GetLabel(int row, int column)
        {
            return (LabelsOnBoard.Find(l => (l.Row == row) && (l.Column == column)));
        }

        void SetGridLabel(int row, int column, NumberLabel Label)
        {
            NumberLabel LabelToAssign = GetLabel(row, column);
            LabelToAssign = Label;
        }

        public event EventHandler AnimationsComplete;

        internal void HighlightCell(Cell cell)
        {
            NumberLabel LabelToHighlight = GetLabel(cell.Row, cell.Column);
            LabelToHighlight.FlashGreen();
        }
        internal void HighlightCell(PositionOnBoard position)
        {
            NumberLabel LabelToHighlight = GetLabel(position.Row, position.Column);
            LabelToHighlight.FlashBlue();
        }

        internal void HighlightCellBase(Cell cell)
        {
            NumberLabel LabelToHighlight = GetLabel(cell.Row, cell.Column);
            LabelToHighlight.FlashRed();
        }
    }
}
