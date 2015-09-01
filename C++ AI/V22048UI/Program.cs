using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Windows.Forms;
using V22048Game.Elements;
using V22048Game.Gameplay;
using System.Reflection;
using V22048Game.MoveInformation;
using V22048UI.UI;

namespace V22048UI
{
    class Program
    {
        static MainWindow MainWindow;

        [STAThread]
        public static void Main()
        {
            App app = new V22048UI.App();
            
            MainWindow = new MainWindow();
            MainWindow.GetGridFromGame += MainWindow_GetGrid;
            //MainWindow.GetMoveFromGame += MainWindow_GetMoveFromGame;
            MainWindow.UndoLastMove += MainWindow_UndoLastMove;
            MainWindow.RestartGame += MainWindow_RestartGame;
            MainWindow.MovesComplete += MainWindow_MovesComplete;
            MainWindow.GameClosed += MainWindow_GameClosed;

            GameController.ScoreChanged += GameController_ScoreChanged;
            GameController.HighestBlockChanged += GameController_HighestBlockChanged;
            GameController.HighScoreChanged += GameController_HighScoreChanged;
            GameController.HighBlockChanged += GameController_HighBlockChanged;
            GameController.AnimationQueueLengthChanged += GameController_AnimationQueueLengthChanged;
            GameController.RandomFixed += GameController_RandomFixed;
            GameController.MoveMade += GameController_MoveMade;
            GameController.GameStarted += GameController_GameStarted;
            GameController.GameOver += GameController_GameOver;
            GameController.StartGamePlayThread();

            app.Run(MainWindow);
        }

        static void MainWindow_GameClosed(object sender, EventArgs e)
        {
            GameController.GameClosed();
        }

        static void MainWindow_MovesComplete(object sender, EventArgs e)
        {
            GameController.MovesComplete();
        }

        static void GameController_AnimationQueueLengthChanged(object sender, int e)
        {
            MainWindow.SetMoveQueueLength(e);
        }

        static void MainWindow_RestartGame(object sender, EventArgs e)
        {
            GameController.RestartGame();
        }

        static void GameController_GameOver(object sender, EventArgs e)
        {
            MainWindow.ShowEndOfGame();
        }

        static void MainWindow_UndoLastMove(object sender, EventArgs e)
        {
            MainWindow.ShowGrid(GameController.UndoMove());
        }

        static void GameController_GameStarted(object sender, EventArgs e)
        {
            MainWindow.ShowGrid(GameController.GetGrid());
        }

        static Board MainWindow_GetGrid()
        {
            return GameController.GetGrid();
        }
        /*static bool MainWindow_GetMoveFromGame(out MoveEventArgs nextMove)
        {
            return GameController.GetAnimationDetails(out nextMove);
        }*/

        static void MainWindow_ClearMoveQueue(object sender, EventArgs e)
        {
            GameController.ClearMoveQueue();
        }

        static void GameController_MoveMade(object sender, MoveEventArgs e)
        {
            MainWindow.RecieveMoveFromGame(e);
        }

        static void GameController_RandomFixed(object sender, bool e)
        {
            MainWindow.SetFixRandom(e);
        }

        static void GameController_ScoreChanged(object sender, int e)
        {
            MainWindow.SetScore(e);
        }

        static void GameController_HighBlockChanged(object sender, int e)
        {
            MainWindow.SetHighBlock(e);
        }

        static void GameController_HighScoreChanged(object sender, int e)
        {
            MainWindow.SetHighScore(e);
        }

        static void GameController_HighestBlockChanged(object sender, int e)
        {
            MainWindow.SetHighestBlock(e);
        }
    }
}
