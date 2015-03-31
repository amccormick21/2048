using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.IO;
using _2048Game.Board;
using _2048Game.AI;
using _2048Game.UI;

namespace _2048Game
{
    public enum DragDirection { Left, Up, Right, Down }
    public static class DragDirections
    {
        public static DragDirection[] AllDragDirections = (DragDirection[])Enum.GetValues(typeof(DragDirection));

        public static DragDirection Opposite(DragDirection Direction)
        {
            int div = 2*(((int)Direction)/2);
            int directionBase = 2 - div;
            int mod = ((int)Direction % 2);
            return (DragDirection)(directionBase + mod);
        }
    }
}

namespace _2048Game.GamePlay
{
    public class Game
    {
        MainWindow parent;
        PlayingGrid grid;
        ArtificialIntelligence AI;
        int score;
        int highestBlock;
        bool canAcceptMoves;
        bool hasMoveQueue;
        bool aiPlay;
        bool movesBeingUndone;
        bool gameOver;
        Queue<DragDirection> MoveQueue;
        Queue<GridRecord> LastGrids;
        Stack<GridRecord> LastGridStack;

        public delegate void GameOverEventHandler(GameOverEventArgs e);
        public event GameOverEventHandler GameOver;

        event EventHandler CannotMove;

        public Game(MainWindow Parent)
        {
            canAcceptMoves = false;
            hasMoveQueue = false;
            aiPlay = false;
            movesBeingUndone = false;
            gameOver = false;
            MoveQueue = new Queue<DragDirection>();
            LastGrids = new Queue<GridRecord>();
            LastGridStack = new Stack<GridRecord>();
            grid = new PlayingGrid();
            this.SetParent(Parent);
            grid.MoveMadeOnGrid += Grid_MoveMadeOnGrid;
            AIPlayChanged += Game_AIPlayChanged;
            parent.MoveBlocks += Parent_MoveBlocks;
            parent.AnimationsComplete += Parent_AnimationsComplete;
            CannotMove += Parent_AnimationsComplete;
            highestBlock = 0;
            GameOver += Game_GameOver;
            Score = 0;
            if (!App.GameRunning)
                StartGame();
            canAcceptMoves = true;
        }

        void Game_AIPlayChanged(object sender, bool e)
        {
            if (e) //AI is playing
            {
                bool waitingForAnimationsToFinish = !canAcceptMoves;
                canAcceptMoves = false;
                MoveQueue.Clear(); //clear the queue to prevent interference
                hasMoveQueue = false;

                //Starts the AI.
                //The animations finished event triggers the AI.GetMove.
                this.AI = new ArtificialIntelligence(this.Grid, this);

                this.AI.PlanNextMove();

                //Makes the first move 
                if (!waitingForAnimationsToFinish)
                {
                    this.AI.FireNextMove();
                }

            }
            else
            {
                canAcceptMoves = true;
            }
        }

        internal void AI_Move(DragDirection Direction)
        {
            if (movesBeingUndone)
            {
                movesBeingUndone = false;
                LastGrids.Clear();
                do
                {
                    LastGrids.Enqueue(LastGridStack.Pop());
                } while (LastGridStack.Count != 0);
            }

            AddMoveToQueue();

            MoveBlocksInDirection(Direction);
        }

        private void SetParent(MainWindow Parent)
        {
            this.Parent = Parent;
            parent.HighScoreBox.Text = Convert.ToString(App.HighScore);
        }

        void Parent_AnimationsComplete(object sender, EventArgs e)
        {
            if (!gameOver)
            {
                if (AIPlay)
                {
                    AI.FireNextMove();
                    AI.PlanNextMove();
                }
                else
                {
                    hasMoveQueue = !(MoveQueue.Count == 0);
                    if (hasMoveQueue)
                    {
                        var MoveDirection = MoveQueue.Dequeue();
                        MoveBlocksInDirection(MoveDirection);
                    }
                    else
                    {
                        canAcceptMoves = true;
                    }
                }
            }
        }

        void Game_GameOver(GameOverEventArgs e)
        {
            canAcceptMoves = false;
            AIPlay = false;
            gameOver = true;
            if (AI != null)
                AI.Move -= AI_Move;
            if (App.HighScore < e.Score)
            {
                App.HighScore = e.Score;
                parent.HighScoreBox.Text = Convert.ToString(App.HighScore);
                WriteStatsToFile();
            }
            App.GameRunning = false;
        }

        private void WriteStatsToFile()
        {
            StreamWriter sw = new StreamWriter("Stats.txt");
            sw.WriteLine(Convert.ToString(App.HighScore));
            sw.WriteLine(Convert.ToString(App.HighestBlock));
            sw.Dispose();
        }

        void Parent_MoveBlocks(DragDirection direction)
        {
            if (movesBeingUndone)
            {
                movesBeingUndone = false;
                LastGrids.Clear();
                do
                {
                    LastGrids.Enqueue(LastGridStack.Pop());
                } while (LastGridStack.Count != 0);
            }

            AddMoveToQueue();

            if (!AIPlay)
            {
                if (canAcceptMoves)
                {
                    MoveBlocksInDirection(direction);
                }
                else
                {
                    MoveQueue.Enqueue(direction);
                }
            }
        }

        internal void MoveBlocksInDirection(DragDirection direction)
        {
            if (Grid.CanMove[direction])
            {
                canAcceptMoves = false;
                if (App.GameRunning)
                {
                    int scoreDelta;
                    bool gameOver;
                    MakeMove(direction, out scoreDelta, out gameOver);
                    Score += scoreDelta;
                    if (scoreDelta > HighestBlock)
                        HighestBlock = scoreDelta;
                    if (gameOver)
                        OnGameOver();
                }
            }
            else
            {
                CannotMove(this, new EventArgs());
            }
        }

        private void AddMoveToQueue()
        {
            LastGrids.Enqueue(new GridRecord(new PlayingGrid(this.Grid), this.Score));

            if (LastGrids.Count > 50)
            {
                LastGrids.Dequeue();
            }
        }

        public void UndoLastMove()
        {
            if (canAcceptMoves)
            {
                canAcceptMoves = false;
                this.Grid.MoveMadeOnGrid -= Grid_MoveMadeOnGrid;

                if (!movesBeingUndone)
                {
                    movesBeingUndone = true;
                    LastGridStack.Clear();
                    do
                    {
                        LastGridStack.Push(LastGrids.Dequeue());
                    } while (LastGrids.Count != 0);
                }

                GridRecord Record = LastGridStack.Pop();

                this.Grid = new PlayingGrid(Record.Grid);
                this.Score = Record.score;

                this.Grid.MoveMadeOnGrid += Grid_MoveMadeOnGrid;
                Parent.ShowGrid(this.Grid);
                canAcceptMoves = true;
            }
        }

        internal void Grid_MoveMadeOnGrid(MoveMadeEventArgs e)
        {
            Parent.UpdateLabelsOnBoard(e);
            App.HighestBlock = this.Grid.GetLargestCellOnGrid().Value;
        }

        /// <summary>
        /// Adds all of the labels to the window and initialises two random cells at a value of 2.
        /// </summary>
        public void StartGame()
        {
            Random rand = new Random();
            int randomRow, randomColumn;
            int cellsAssigned = 0;

            do
            {
                randomRow = rand.Next(0, 4);
                randomColumn = rand.Next(0, 4);
                if (Grid[randomRow, randomColumn] != 2)
                {
                    Grid[randomRow, randomColumn] = 2;
                    Parent.AddLabel(new NumberLabel(Grid.Cells[randomRow, randomColumn], Parent.Board.Width / 4, Parent.Board.Height / 4), randomRow, randomColumn);
                    cellsAssigned++;
                }
            } while (cellsAssigned < 2);

            Grid.SetCanSizeInDirection();

            App.GameRunning = true;
        }

        public void MakeMove(DragDirection direction, out int scoreDelta, out bool gameOver)
        {
            gameOver = grid.MakeMove(direction, out scoreDelta);
        }

        internal void OnGameOver()
        {
            if (GameOver != null)
                GameOver(new GameOverEventArgs(Score, HighestBlock));
        }

        public MainWindow Parent
        {
            get { return parent; }
            set { parent = value; }
        }
        public PlayingGrid Grid
        {
            get { return grid; }
            set { grid = value; }
        }

        public bool AIPlay
        {
            get { return aiPlay; }
            set
            {
                if (aiPlay != value)
                {
                    aiPlay = value;
                    if (AIPlayChanged != null)
                    {
                        AIPlayChanged(this, aiPlay);
                    }
                }
            }
        }

        public event EventHandler<bool> AIPlayChanged;

        public int Score
        {
            get { return score; }
            set { score = value; parent.SetScore(score); }
        }
        public int HighestBlock
        {
            get { return highestBlock; }
            set { highestBlock = value; }
        }

        internal void TreeFound(object sender, CellTree e)
        {
            foreach (TreeNode node in e.NodesInChain)
            {
                //Parent.HighlightCell(node.Position);
            }
        }
    }
}
