using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using V22048Game.Elements;
using V22048Game.GameRules;
using V22048Game.MoveInformation;

namespace V22048Game.Gameplay
{
    public static class GameController
    {
        public const int GridSize = 4;
        public const int SpawnCells = 1;

        public static bool GameRunning { get; private set; }
        public static bool FixRandom { get; private set; }
        public static bool AIPlaying { get; private set; }

        static Game game;
        static Records records;

        public static IGameRules GameRules = new StandardGameRules();
        public static IAi AI;

        /// <summary>
        /// Initialises the 2048 game processes and sets up the board.
        /// </summary>
        public static void StartGamePlayThread()
        {
            //Set the fix random value to the default starting value: true.
            StartGamePlay();
        }

        private static void StartGamePlay()
        {
            FixRandom = true;

            records = new Records();
            records.HighestBlockChanged += (s, e) => HighestBlockChanged(s, e);
            records.HighScoreChanged += (s, e) => HighScoreChanged(s, e);
            records.ScoreChanged += (s, e) => ScoreChanged(s, e);
            records.HighBlockChanged += (s, e) => HighBlockChanged(s, e);
            records.Init();

            game = new Game();
            game.ScoreChanged += (s, e) => records.SetScore(e);
            game.HighBlockChanged += (s, e) => records.SetHighBlock(e);
            game.MoveMade += (s, e) => MoveMade(s, e);
            game.AnimationQueueLengthChanged += (s, e) => AnimationQueueLengthChanged(s, e);
            game.RandomFixed += (s, e) => RandomFixed(s, e);
            game.GameOver += game_GameOver;
            game.BoardInitialised += game_BoardInitialised;
            game.Init(fixRandom: FixRandom);

            SetGameRunning(true);
        }

        //Interactions with UI:
        public static event EventHandler GameStarted;
        public static event EventHandler GameOver;
        public static event EventHandler<int> HighestBlockChanged;
        public static event EventHandler<int> HighScoreChanged;
        public static event EventHandler<int> ScoreChanged;
        public static event EventHandler<int> HighBlockChanged;
        public static event EventHandler<int> AnimationQueueLengthChanged;
        public static event EventHandler<bool> RandomFixed;
        public static event EventHandler<MoveEventArgs> MoveMade;

        /// <summary>
        /// Sets the game running flag to true when the game has finished initialising the board.
        /// </summary>
        static void game_BoardInitialised(object sender, Board e)
        {
            SetGameRunning(true);
        }

        static void game_GameOver(object sender, EventArgs e)
        {
            records.GameOver();
            //This also fires the game over event.
            SetGameRunning(false);
        }

        public static void Move(Direction directionOfMove)
        {
            game.Move(directionOfMove);
        }

        public static void SetGameRunning(bool gameRunning)
        {
            GameRunning = gameRunning;

            if (!gameRunning)
            {
                AIPlaying = false;
            }

            if (GameStarted != null && gameRunning)
                GameStarted(null, new EventArgs());
            else
                GameOver(null, new EventArgs());
        }

        public static void SetFixRandom(bool fixRandom)
        {
            FixRandom = fixRandom;
            game.SetFixRandom(fixRandom);
        }

        public static void ClearMoveQueue()
        {
            game.ClearMoveQueue();
        }

        public static void ClearAnimationQueue()
        {
            game.ClearAnimationQueue();
        }

        public static Board GetGrid()
        {
            return game.GetGrid();
        }

        public static void MovesComplete()
        {
            game.MovesComplete();
        }

        /*public static bool GetAnimationDetails(out MoveEventArgs nextMove)
        {
            return game.GetMove(out nextMove);
        }*/

        public static Board UndoMove()
        {
            return game.UndoMove();
        }

        public static void RestartGame()
        {
            records.GameOver();
            game.Init(fixRandom:FixRandom);
            SetGameRunning(true);
        }

        public static void GameClosed()
        {
            records.GameOver();
        }

        #region AI Integration

        public static void StartAI(IAi Ai)
        {
            AI = Ai;
            AIPlaying = true;

            //Subscribe to events
            //game.BoardReadyForNextMove += game_BoardDistancesAnalysed;
            game.GetNextAIMove += game_BoardReadyForNextMove;
            game.AIPlaying = true;

            //Set the first move going
            GetAIMove(game.GetGrid());
            game.Move(MakeAIMove());
        }

        public static void StopAI()
        {
            AIPlaying = false;
            //game.BoardReadyForNextMove -= game_BoardDistancesAnalysed;
            game.GetNextAIMove -= game_BoardReadyForNextMove;
            game.AIPlaying = false;
            ClearMoveQueue();
        }

        static void game_BoardDistancesAnalysed(object sender, Board e)
        {
            //Uncomment if returning to background processing
            var nextGrid = e;
            GetAIMove(e);
        }

        static void game_BoardReadyForNextMove(object sender, Board e)
        {
            //Designed to run asynchronously here. Comment if returning to background processing
            //var nextGrid = e;
            GetAIMove(e);
            var direction = MakeAIMove();
            Move(direction);
        }

        static void GetAIMove(Board nextGrid)
        {
            AI.PrepareMove(nextGrid);
        }

        internal static Direction MakeAIMove()
        {
            var nextMove = AI.GetPreparedMove();
            return nextMove;
        }
        #endregion
    }
}
