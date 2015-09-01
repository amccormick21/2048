using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using V22048Game.Elements;
using V22048Game.MoveInformation;

namespace V22048Game.Gameplay
{
    class Game
    {
        Board board;
        bool canAcceptMoves;
        bool gameOver;
        GuardedQueue<Direction> MoveQueue;
        GuardedQueue<MoveEventArgs> AnimationsQueue;

        Stack<GridRecord> lastGrids;

        int score;
        int Score
        {
            get { return score; }
            set { score = value; ScoreChanged(this, score); }
        }
        int highBlock;
        int HighBlock
        {
            get { return highBlock; }
            set { highBlock = value; HighBlockChanged(this, highBlock); }
        }

        int moveQueueLength;
        int MoveQueueLength
        {
            get { return moveQueueLength; }
            set { moveQueueLength = value; }
        }

        bool aiPlaying;
        internal bool AIPlaying
        {
            get { return aiPlaying; }
            set { aiPlaying = value; }
        }

        int animationQueueLength;
        internal event EventHandler<int> AnimationQueueLengthChanged;
        int AnimationQueueLength
        {
            get { return animationQueueLength; }
            set
            {
                animationQueueLength = value;
                if (AnimationQueueLengthChanged != null)
                    AnimationQueueLengthChanged(this, value);
            }
        }

        private event EventHandler GetNextMove;
        internal event EventHandler<Board> GetNextAIMove;

        public Game()
        {
            canAcceptMoves = false;
            MoveQueue = new GuardedQueue<Direction>(ImplementMoveOnGrid);
            AnimationsQueue = new GuardedQueue<MoveEventArgs>(MoveEventArgsReleased);
        }

        internal void Init(bool fixRandom)
        {
            board = new Board(GameController.GridSize);
            //board.MoveDistancesAnalysed += board_MoveDistancesAnalysed;
            board.RandomFixed += board_RandomFixed;
            lastGrids = new Stack<GridRecord>();
            this.GetNextMove += Game_GetNextMove;
            board.PrepareGridForStart(GameController.SpawnCells * 2, fixRandom);
            BoardInitialised(this, board);
            Score = 0;
            HighBlock = 0;
            MoveQueueLength = 0;
            AnimationQueueLength = 0;
            gameOver = false;
            canAcceptMoves = true;
        }

        void Game_GetNextMove(object sender, EventArgs e)
        {
            if (BoardDistancesAnalysed != null)
                BoardDistancesAnalysed(this, board);
            //Unlocking the move queue will cause the next move to be generated
            MoveQueue.Unlock();
        }

        void board_RandomFixed(object sender, bool e)
        {
            RandomFixed(this, e);
        }

        /*void board_MoveDistancesAnalysed(object sender, EventArgs e)
        {
            if (BoardDistancesAnalysed != null)
                BoardDistancesAnalysed(this, board);
            //We are ready to play a new move on the grid
            ManageMoveQueue();
        }

        private void ManageMoveQueue()
        {
            if (MoveQueueLength == 0)
            {
                canAcceptMoves = true;
            }
            else
            {
                Direction nextMove;
                //Keep dequeueing until we find a move we can actually make
                do
                {
                    nextMove = MoveQueue.Dequeue();
                    MoveQueueLength--;
                }
                while (!board.CanMove(nextMove));

                ImplementMoveOnGrid(nextMove);
            }
        }*/

        internal void MovesComplete()
        {
            //This causes the next animation to be triggered
            AnimationsQueue.Unlock();

            //If the AI is playing, it can plan its next move now
            if (AIPlaying && GetNextAIMove != null)
                GetNextAIMove(this, this.GetGrid());
        }

        internal void Move(Direction directionToMove)
        {
            if (!gameOver)
            {
                MoveQueue.Enqueue(directionToMove);
                MoveQueueLength++;
            }
        }

        #region MoveProcessing
        private void ImplementMoveOnGrid(Direction directionToMove)
        {
            //This means that a move has been dequeued
            MoveQueueLength--;
            if (board.CanMove(directionToMove))
            {
                lastGrids.Push(new GridRecord(new Board(this.board), Score, HighBlock));
                var moveProcessing = DoMoveProcessing(directionToMove);

                var nextMoveEventArgs = moveProcessing.NextMoveEventArgs;
                AnimationsQueue.Enqueue(nextMoveEventArgs);
                AnimationQueueLength++;

                int scoreDelta = moveProcessing.ScoreDelta;
                int highestBlockCreated = moveProcessing.HighestBlock;
                bool canStillMove = moveProcessing.CanStillMove;

                Score += scoreDelta;
                if (highestBlockCreated > HighBlock)
                {
                    HighBlock = highestBlockCreated;
                }
                if (!canStillMove)
                {
                    DoGameOver();
                }

                //Now do graphics.
                /*if (nextMoveEventArgs.Moves.Count != 0)
                {
                    MoveMade(this, new EventArgs());
                }*/
            }
            if (this.GetNextMove != null)
                this.GetNextMove(this, new EventArgs());

            /*if (canStillMove)
            {
                if (BoardReadyForNextMove != null)
                    BoardReadyForNextMove(this, board);
            }*/
        }

        private void MoveEventArgsReleased(MoveEventArgs args)
        {
            AnimationQueueLength--;
            //Lock the queue to prevent instant generation of further animations
            //It will be unlocked after the animation is completed
            if (args.Moves.Count != 0 && MoveMade != null)
                MoveMade(this, args);
        }

        private void DoGameOver()
        {
            canAcceptMoves = false;
            gameOver = true;
            MoveQueue.Clear();
            GameOver(this, new EventArgs());
        }

        private MoveResults DoMoveProcessing(Direction directionOfNextMove)
        {
            MoveEventArgs nextMoveEventArgs;
            int scoreDelta;
            int highestBlockCreated;
            bool endOfGame;

            nextMoveEventArgs = board.Move(directionOfNextMove, out scoreDelta, out highestBlockCreated);

            if (nextMoveEventArgs.Moves.Count != 0)
            {
                nextMoveEventArgs.NewCells = board.InsertRandomNewCells(GameController.SpawnCells);
                endOfGame = board.AnalyseNextMoveDistances();
                //board.WriteGrid();
            }
            else
            {
                endOfGame = board.AnalyseNextMoveDistances();
            }
            MoveResults moveResults = new MoveResults();
            moveResults.NextMoveEventArgs = nextMoveEventArgs;
            moveResults.ScoreDelta = scoreDelta;
            moveResults.HighestBlock = highestBlockCreated;
            moveResults.CanStillMove = endOfGame;
            return moveResults;
        }
        #endregion

        internal void SetFixRandom(bool fixRandom)
        {
            board.SetFixRandom(fixRandom);
        }

        internal void ClearMoveQueue()
        {
            MoveQueue.Clear();
            MoveQueueLength = 0;
        }
        internal void ClearAnimationQueue()
        {
            AnimationsQueue.Clear();
            AnimationQueueLength = 0;
        }

        internal event EventHandler<Board> BoardInitialised;
        internal event EventHandler<Board> BoardReadyForNextMove;
        internal event EventHandler<Board> BoardDistancesAnalysed;
        internal event EventHandler<int> ScoreChanged;
        internal event EventHandler<int> HighBlockChanged;
        internal event EventHandler<MoveEventArgs> MoveMade;
        internal event EventHandler<bool> RandomFixed;
        internal event EventHandler GameOver;

        internal Board GetGrid()
        {
            return board;
        }

        /*
        internal bool GetMove(out MoveEventArgs MoveToMake)
        {
            if (aiPlaying)
            {
                if (BoardReadyForNextMove != null)
                    BoardReadyForNextMove(this, board);

                ImplementMoveOnGrid(GameController.MakeAIMove());
            }

            if (AnimationQueueLength == 0)
            {
                MoveToMake = null;
                return false;
            }
            else
            {
                AnimationQueueLength--;
                MoveToMake = AnimationsQueue.Dequeue();
                return true;
            }
        }*/

        internal Board UndoMove()
        {
            GridRecord lastGridRecord = lastGrids.Pop();
            this.board = new Board(lastGridRecord.Board);
            Score = lastGridRecord.Score;
            HighBlock = lastGridRecord.HighBlock;
            gameOver = false;
            canAcceptMoves = true;
            return this.board;
        }
    }
}
