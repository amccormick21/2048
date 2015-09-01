using DeterministicAI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Reflection;
using V22048Game.Elements;
using V22048Game.Gameplay;
using V22048Game.MoveInformation;
using V22048UI.Animations;

namespace V22048UI.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<NumberLabel> LabelsOnBoard;
        //bool moving;
        double SpeedOfAnimations;

        internal event EventHandler MovesComplete;
        internal event EventHandler GameClosed;

        public MainWindow()
        {
            InitializeComponent();
            InitialiseLabels();
            //moving = false;

            this.Closed += MainWindow_Closed;
            Restart.MouseDown += Restart_MouseDown;
            Undo.MouseDown += Undo_MouseDown;
            ResetQueue.Click += ResetQueue_MouseDown;
            AI.Click += AI_Click;
            AIPlay += MainWindow_AIPlay;
            AISpeed.ValueChanged += AISpeed_ValueChanged;
            FixedRandom.Checked += FixedRandom_Checked;
            AISpeed.Value = 9.2;
            FixedRandom.IsChecked = GameController.FixRandom;

            if (GameController.GridSize < 50)
                GridBorder.CornerRadius = new CornerRadius(10 - (Math.Sqrt(GameController.GridSize)));
            else
                GridBorder.CornerRadius = new CornerRadius(0);

            this.PreviewKeyDown += MainWindow_PreviewKeyDown;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            //Same effect as a game over
            if (this.GameClosed != null)
                this.GameClosed(this, new EventArgs());
        }

        void MainWindow_AIPlay(object sender, bool e)
        {
            if (e)
                GameController.StartAI(new Deterministic2048AI());
            else
                GameController.StopAI();
        }

        void FixedRandom_Checked(object sender, RoutedEventArgs e)
        {
            if (FixedRandom.IsChecked.HasValue)
            {
                GameController.SetFixRandom((bool)FixedRandom.IsChecked.Value);
            }
        }

        void ResetQueue_MouseDown(object sender, EventArgs e)
        {
            ClearMoveQueue();
        }

        void AISpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SpeedOfAnimations = e.NewValue;
            AISpeed.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
        }

        public event EventHandler<bool> AIPlay;
        void AI_Click(object sender, RoutedEventArgs e)
        {
            //Setting AIPlay fires an event which starts the AI in the game
            if (AI.IsChecked.HasValue)
            {
                if (AIPlay != null)
                    AIPlay(this, (bool)AI.IsChecked);
            }
            else
            {
                if (AIPlay != null)
                    AIPlay(this, false);
            }
        }

        private void InitialiseLabels()
        {
            LabelsOnBoard = new List<NumberLabel>();
        }

        public event EventHandler UndoLastMove;
        void Undo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearMoveQueue();
            HideEndOfGame();
            //if (!moving)
            //{
            if (UndoLastMove != null)
                UndoLastMove(this, new EventArgs());
            //}
        }

        public event EventHandler RestartGame;
        public void Restart_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ClearMoveQueue();
            HideEndOfGame();
            ClearGrid();
            RestartGame(this, new EventArgs());
        }

        delegate void ProcessHandler();
        internal void ClearGrid()
        {
            if (Dispatcher.CheckAccess())
            {
                Board.Children.Clear();
                Board.Children.Add(LoseLabel);
            }
            else
            {
                Dispatcher.Invoke(new ProcessHandler(ClearGrid));
            }
        }

        public void AddLabel(NumberLabel label)
        {
            LabelsOnBoard.Add(label);
            Board.Children.Add(label);
            Canvas.SetLeft(label, GetGridPosition(label.Column, true));
            Canvas.SetTop(label, GetGridPosition(label.Row, false));
        }

        void MainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left: GameController.Move(Direction.Left); break;
                case Key.Up: GameController.Move(Direction.Up); break;
                case Key.Right: GameController.Move(Direction.Right); break;
                case Key.Down: GameController.Move(Direction.Down); break;
                default: break;
            }
        }

        internal void ShowEndOfGame()
        {
            if (Dispatcher.CheckAccess())
            {
                if (!GameController.AIPlaying)
                    LoseLabel.Visibility = System.Windows.Visibility.Visible;
                Canvas.SetZIndex(LoseLabel, LabelsOnBoard.Count);
            }
            else
            {
                Dispatcher.Invoke(new ProcessHandler(ShowEndOfGame));
            }
        }
        internal void HideEndOfGame()
        {
            if (Dispatcher.CheckAccess())
            {
                LoseLabel.Visibility = System.Windows.Visibility.Hidden;
            }
            else
            {
                Dispatcher.Invoke(new ProcessHandler(HideEndOfGame));
            }
        }

        delegate void ShowGridHandler(Board board);
        internal void ShowGrid(Board board)
        {
            if (Dispatcher.CheckAccess())
            {
                ClearGrid();
                LabelsOnBoard.Clear();
                for (int row = 0; row < GameController.GridSize; row++)
                {
                    for (int column = 0; column < GameController.GridSize; column++)
                    {
                        if (!board[row, column].Value.Equals(GameController.GameRules.GetEmptyCellValue()))
                        {
                            AddLabel(new NumberLabel(board[row, column], Board.Width / (double)GameController.GridSize, Board.Height / (double)GameController.GridSize, false));
                        }
                    }
                }
            }
            else
            {
                Dispatcher.Invoke(new ShowGridHandler(ShowGrid), new object[] { board });
            }
        }

        /// <summary>
        /// Recieves a move from the game classes and either queues it or runs it.
        /// </summary>
        public void RecieveMoveFromGame(MoveEventArgs e)
        {
            //We have got here because the game has released a move
            //This was caused by the previous one being completed, so we can go ahead and do it.
            MakeMove(e);
        }

        /*
        /// <summary>
        /// Deals with the move queue when an animation has been completed.
        /// Either opens the doors for new variables to come straight in, or continues to remove some from the queue.
        /// </summary>
        private void ManageMoveQueue()
        {
            //We have got to here because an animation has been completed.
            //We can now ask for another move.
            //Moving will always be true as we enter here.
            if (Dispatcher.CheckAccess())
            {
                MoveEventArgs nextMove;
                if (GetMove(out nextMove))
                {
                    MakeMove(nextMove);
                }
                else
                {
                    //No moves available in the queue
                    //moving = false;
                    //The recursion scales backwards here and unwinds.
                }
            }
            else
            {
                Dispatcher.Invoke(new ProcessHandler(ManageMoveQueue));
            }
        }*/

        private void MakeMove(MoveEventArgs e)
        {
            //Deal with the event args
            //moving = true; //As soon as we start setting this guard, the thing fails. So what's going on?
            //TODO.
            if (GameController.GridSize > 15)
            {
                ShowGrid(GetGridFromGame());
                if (MovesComplete != null)
                    MovesComplete(this, new EventArgs());
                //ManageMoveQueue();
            }
            else
            {
                MoveAnimation Animation = new MoveAnimation(LabelsOnBoard, SpeedOfAnimations, Board);
                Animation.AllAnimationsComplete += Animation_AllAnimationsComplete;
                Animation.SetupAnimations(e);
                Animation.StartAnimations();
            }
        }

        void Animation_AllAnimationsComplete(object sender, EventArgs e)
        {
            //ManageMoveQueue();
            if (MovesComplete != null)
                MovesComplete(this, new EventArgs());
        }

        internal delegate Board GetGameGrid();
        internal event GetGameGrid GetGridFromGame;
        private Board GetGrid()
        {
            return GetGridFromGame();
        }
        /*
        internal delegate bool GetMoveEventHandler(out MoveEventArgs nextMove);
        internal event GetMoveEventHandler GetMoveFromGame;
        private bool GetMove(out MoveEventArgs nextMove)
        {
            return GetMoveFromGame(out nextMove);
        }*/

        NumberLabel GetLabel(Position position)
        {
            return (LabelsOnBoard.Find(l => (l.Row == position.Row) && (l.Column == position.Column)));
        }

        double GetGridPosition(int index, bool isLeftPosition)
        {
            return index * ((isLeftPosition ? Board.Width : Board.Height) / GameController.GridSize);
        }

        void SetGridLabel(Position position, NumberLabel Label)
        {
            NumberLabel LabelToAssign = GetLabel(position);
            LabelToAssign = Label;
        }

        delegate void SetIntParameter(int value);
        delegate void SetBoolParameter(bool value);
        internal void SetScore(int score)
        {
            if (Dispatcher.CheckAccess())
            {
                ScoreBox.Text = Convert.ToString(score);
            }
            else
            {
                Dispatcher.Invoke(new SetIntParameter(SetScore), new object[] { score });
            }
        }
        internal void SetHighBlock(int highBlock)
        {
            if (Dispatcher.CheckAccess())
            {
                HighBlockBox.Text = Convert.ToString(highBlock);
            }
            else
            {
                Dispatcher.Invoke(new SetIntParameter(SetHighBlock), new object[] { highBlock });
            }
        }
        internal void SetHighScore(int highScore)
        {
            if (Dispatcher.CheckAccess())
            {
                HighScoreBox.Text = Convert.ToString(highScore);
            }
            else
            {
                Dispatcher.Invoke(new SetIntParameter(SetHighScore), new object[] { highScore });
            }
        }
        internal void SetHighestBlock(int highestBlock)
        {
            if (Dispatcher.CheckAccess())
            {
                HighestBlockBox.Text = Convert.ToString(highestBlock);
            }
            else
            {
                Dispatcher.Invoke(new SetIntParameter(SetHighestBlock), new object[] { highestBlock });
            }
        }
        internal void SetFixRandom(bool fixRandom)
        {
            if (Dispatcher.CheckAccess())
            {
                FixedRandom.Checked -= FixedRandom_Checked;
                FixedRandom.IsChecked = fixRandom;
                FixedRandom.Checked += FixedRandom_Checked;
            }
            else
            {
                Dispatcher.Invoke(new SetBoolParameter(SetFixRandom), new object[] { fixRandom });
            }
        }
        internal void SetMoveQueueLength(int moveQueueLength)
        {
            if (Dispatcher.CheckAccess())
            {
                MoveQueueLengthBox.Text = Convert.ToString(moveQueueLength);
            }
            else
            {
                Dispatcher.Invoke(new SetIntParameter(SetMoveQueueLength), new object[] { moveQueueLength });
            }
        }
        private void ClearMoveQueue()
        {
            GameController.ClearMoveQueue();
            GameController.ClearAnimationQueue();
        }
    }
}
