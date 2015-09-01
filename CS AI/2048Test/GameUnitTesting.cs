using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using _2048Game;
using _2048Game.UI;
using _2048Game.GamePlay;
using _2048Game.Board;

namespace _2048Test
{
    [TestClass]
    public class GameUnitTesting
    {
        [TestMethod]
        [TestCategory("Game Unit Testing")]
        public void TestSetScaleInDirection()
        {
            PlayingGrid grid = new PlayingGrid();
            List<Cell> SetCells = new List<Cell> { new Cell(0, 0, 2), new Cell(0, 2, 4), new Cell(1, 0, 2), new Cell(2, 3, 4) };
            List<int> ExpectedMaxScales = new List<int> { 2, 1, 3, 0 };
            GetGrid(ref grid, SetCells);
            bool canMove = grid.SetCanSizeInDirection(DragDirection.Right);

            for (int i = 0; i < SetCells.Count; i++)
            {
                Assert.AreEqual(ExpectedMaxScales[i], grid.Cells[SetCells[i].Row, SetCells[i].Column].MaxScaleInDirection[DragDirection.Right]);
            }
        }

        [TestMethod]
        [TestCategory("Game Unit Testing")]
        public void TestSetScaleInDirectionCannotMove()
        {
            PlayingGrid grid = new PlayingGrid();
            List<Cell> SetCells = new List<Cell> { new Cell(0, 0, 2), new Cell(0, 1, 4), new Cell(1, 0, 2), new Cell(2, 0, 4) };
            List<int> ExpectedMaxScales = new List<int> { 0, 0, 0, 0 };
            GetGrid(ref grid, SetCells);
            bool canMove = grid.SetCanSizeInDirection(DragDirection.Left);

            Assert.IsFalse(canMove);
            for (int i = 0; i < SetCells.Count; i++)
            {
                Assert.AreEqual(ExpectedMaxScales[i], grid.Cells[SetCells[i].Row, SetCells[i].Column].MaxScaleInDirection[DragDirection.Left]);
            }
        }

        [TestMethod]
        [TestCategory("Game Unit Testing")]
        public void TestSetEmptyCells()
        {
            PlayingGrid grid = new PlayingGrid();
            List<Cell> SetCells = new List<Cell> { new Cell(0, 0, 2), new Cell(0, 1, 4), new Cell(1, 0, 2), new Cell(2, 0, 4) };
            List<int> ExpectedMaxScales = new List<int> { 0, 0, 0, 0 };
            GetGrid(ref grid, SetCells);

            int numberOfEmptyCells = grid.CountEmptyCells();

            Assert.AreEqual(16 - SetCells.Count, numberOfEmptyCells);
        }

        [TestMethod]
        [TestCategory("Game Unit Testing")]
        public void TestMoveCellsLeft()
        {
            PlayingGrid grid = new PlayingGrid();
            List<Cell> SetCells = new List<Cell> { new Cell(0, 0, 2), new Cell(1, 2, 4), new Cell(3, 3, 8) };

            GetGrid(ref grid, SetCells);
            grid.Cells[SetCells[0].Row, SetCells[0].Column].MaxScaleInDirection[DragDirection.Left] = 0;
            grid.Cells[SetCells[1].Row, SetCells[1].Column].MaxScaleInDirection[DragDirection.Left] = 2;
            grid.Cells[SetCells[2].Row, SetCells[2].Column].MaxScaleInDirection[DragDirection.Left] = 3;

            List<Move> Moves;
            List<PositionOnBoard> ValuesChanged;
            int ScoreDelta = 0;
            Moves = grid.MoveAllCells(DragDirection.Left, out ValuesChanged, ref ScoreDelta);

            Assert.AreEqual(2, grid[0, 0]);
            Assert.AreEqual(4, grid[1, 0]);
            Assert.AreEqual(8, grid[3, 0]);
        }
        [TestMethod]
        [TestCategory("Game Unit Testing")]
        public void TestMoveCellsLeftListOfMoves()
        {
            PlayingGrid grid = new PlayingGrid();
            List<Cell> SetCells = new List<Cell> { new Cell(0, 0, 2), new Cell(1, 2, 4), new Cell(3, 3, 8) };

            GetGrid(ref grid, SetCells);
            grid.Cells[SetCells[0].Row, SetCells[0].Column].MaxScaleInDirection[DragDirection.Left] = 0;
            grid.Cells[SetCells[1].Row, SetCells[1].Column].MaxScaleInDirection[DragDirection.Left] = 2;
            grid.Cells[SetCells[2].Row, SetCells[2].Column].MaxScaleInDirection[DragDirection.Left] = 3;

            List<Move> Moves;
            List<PositionOnBoard> ValuesChanged;
            int ScoreDelta = 0;
            Moves = grid.MoveAllCells(DragDirection.Left, out ValuesChanged, ref ScoreDelta);

            Assert.AreEqual(new Move(1,2,1,0), Moves[0]);
            Assert.AreEqual(new Move(3,3,3,0), Moves[1]);
        }
        [TestMethod]
        [TestCategory("Game Unit Testing")]
        public void TestMergeCellsRightValuesChanged()
        {
            PlayingGrid grid = new PlayingGrid();
            List<Cell> SetCells = new List<Cell> { new Cell(0, 0, 2), new Cell(0, 1, 2), new Cell(1, 2, 4), new Cell(3, 3, 8) };

            GetGrid(ref grid, SetCells);
            grid.Cells[SetCells[0].Row, SetCells[0].Column].MaxScaleInDirection[DragDirection.Right] = 3;
            grid.Cells[SetCells[1].Row, SetCells[1].Column].MaxScaleInDirection[DragDirection.Right] = 2;
            grid.Cells[SetCells[2].Row, SetCells[2].Column].MaxScaleInDirection[DragDirection.Right] = 1;
            grid.Cells[SetCells[3].Row, SetCells[3].Column].MaxScaleInDirection[DragDirection.Right] = 0;

            List<Move> Moves;
            List<PositionOnBoard> ValuesChanged;
            int ScoreDelta = 0;
            Moves = grid.MoveAllCells(DragDirection.Left, out ValuesChanged, ref ScoreDelta);

            Assert.AreEqual(new PositionOnBoard(0,0), ValuesChanged[0]);
        }
        [TestMethod]
        [TestCategory("Game Unit Testing")]
        public void TestMergeCellsRightScoreDelta()
        {
            PlayingGrid grid = new PlayingGrid();
            List<Cell> SetCells = new List<Cell> { new Cell(0, 0, 2), new Cell(0, 1, 2), new Cell(1, 2, 4), new Cell(3, 3, 8) };

            GetGrid(ref grid, SetCells);
            grid.Cells[SetCells[0].Row, SetCells[0].Column].MaxScaleInDirection[DragDirection.Right] = 3;
            grid.Cells[SetCells[1].Row, SetCells[1].Column].MaxScaleInDirection[DragDirection.Right] = 2;
            grid.Cells[SetCells[2].Row, SetCells[2].Column].MaxScaleInDirection[DragDirection.Right] = 1;
            grid.Cells[SetCells[3].Row, SetCells[3].Column].MaxScaleInDirection[DragDirection.Right] = 0;

            List<Move> Moves;
            List<PositionOnBoard> ValuesChanged;
            int ScoreDelta = 0;
            Moves = grid.MoveAllCells(DragDirection.Left, out ValuesChanged, ref ScoreDelta);

            Assert.AreEqual(4, ScoreDelta);
        }
        [TestMethod]
        [TestCategory("Game Unit Testing")]
        public void TestMoveCellsDown()
        {
            PlayingGrid grid = new PlayingGrid();
            List<Cell> SetCells = new List<Cell> { new Cell(0, 0, 2), new Cell(1, 2, 4), new Cell(3, 3, 8) };

            GetGrid(ref grid, SetCells);
            grid.Cells[SetCells[0].Row, SetCells[0].Column].MaxScaleInDirection[DragDirection.Down] = 3;
            grid.Cells[SetCells[1].Row, SetCells[1].Column].MaxScaleInDirection[DragDirection.Down] = 2;
            grid.Cells[SetCells[2].Row, SetCells[2].Column].MaxScaleInDirection[DragDirection.Down] = 0;

            List<Move> Moves;
            List<PositionOnBoard> ValuesChanged;
            int ScoreDelta = 0;
            Moves = grid.MoveAllCells(DragDirection.Down, out ValuesChanged, ref ScoreDelta);


            Assert.AreEqual(2, grid[3, 0]);
            Assert.AreEqual(4, grid[3, 2]);
            Assert.AreEqual(8, grid[3, 3]);
        }

        [TestMethod]
        [TestCategory("Game Unit Testing")]
        public void TestMergeCellsAloneOnGridUp()
        {
            PlayingGrid grid = new PlayingGrid();
            List<Cell> SetCells = new List<Cell> { new Cell(0, 0, 2), new Cell(1, 0, 2) };

            GetGrid(ref grid, SetCells);

            List<Move> Moves;
            List<PositionOnBoard> ValuesChanged;
            int scoreDelta = 0;
            Moves = grid.MoveAllCells(DragDirection.Up, out ValuesChanged, ref scoreDelta);

            Assert.AreEqual(4, grid[0, 0]);
            Assert.AreEqual(new Move(1, 0, 0, 0), Moves[0]);
            Assert.AreEqual(new PositionOnBoard(0, 0), ValuesChanged[0]);
            Assert.AreEqual(4, scoreDelta);
        }
        [TestMethod]
        [TestCategory("Game Unit Testing")]
        public void TestMergeCellsSeparatedAloneOnGridRight()
        {
            PlayingGrid grid = new PlayingGrid();
            List<Cell> SetCells = new List<Cell> { new Cell(0, 1, 2), new Cell(0, 3, 2) };

            GetGrid(ref grid, SetCells);

            List<Move> Moves;
            List<PositionOnBoard> ValuesChanged;
            int scoreDelta = 0;
            Moves = grid.MoveAllCells(DragDirection.Right, out ValuesChanged, ref scoreDelta);

            Assert.AreEqual(4, grid[0, 3]);
            Assert.AreEqual(new Move(0,1,0,3), Moves[0]);
            Assert.AreEqual(new PositionOnBoard(0, 3), ValuesChanged[0]);
            Assert.AreEqual(4, scoreDelta);

        }
        [TestMethod]
        [TestCategory("Game Unit Testing")]
        public void TestMergeCellsInterferenceOnGrid()
        {
            PlayingGrid grid = new PlayingGrid();
            List<Cell> SetCells = new List<Cell> { new Cell(0, 0, 4), new Cell(0, 1, 2), new Cell(0, 2, 2), new Cell(0, 3, 8) };
            List<int> ExpectedValues = new List<int> { 4, 4, 8, 0 };

            GetGrid(ref grid, SetCells);

            int scoreDelta = 0;
            List<Move> Moves;
            List<PositionOnBoard> ValuesChanged;
            Moves = grid.MoveAllCells(DragDirection.Left, out ValuesChanged, ref scoreDelta);

            for (int column = 0; column < 4; column++)
            {
                Assert.AreEqual(ExpectedValues[column], grid[0, column]);
            }
        }
        [TestMethod]
        [TestCategory("Game Unit Testing")]
        public void TestMergeCellMultipleMerges()
        {
            PlayingGrid grid = new PlayingGrid();
            List<Cell> SetCells = new List<Cell> { new Cell(0, 0, 2), new Cell(0, 1, 2), new Cell(0, 2, 4), new Cell(0, 3, 4) };
            List<int> ExpectedValues = new List<int> { 4, 8, 0, 0 };

            GetGrid(ref grid, SetCells);

            List<Move> Moves;
            List<PositionOnBoard> ValuesChanged;
            int scoreDelta = 0;
            Moves = grid.MoveAllCells(DragDirection.Left, out ValuesChanged, ref scoreDelta);

            for (int column = 0; column < 4; column++)
            {
                Assert.AreEqual(ExpectedValues[column], grid[0, column]);
            }
            Assert.AreEqual(new Move(0, 1, 0, 0), Moves[0]);
            Assert.AreEqual(new Move(0, 2, 0, 1), Moves[1]);
            Assert.AreEqual(new Move(0, 3, 0, 1), Moves[2]);

            Assert.AreEqual(new PositionOnBoard(0, 0), ValuesChanged[0]);
            Assert.AreEqual(new PositionOnBoard(0, 1), ValuesChanged[1]);

            Assert.AreEqual(12, scoreDelta);
        }
        [TestMethod]
        [TestCategory("Game Unit Testing")]
        public void TestMergeCellNoMerges()
        {
            PlayingGrid grid = new PlayingGrid();
            List<Cell> SetCells = new List<Cell> { new Cell(0, 0, 2), new Cell(0, 1, 8), new Cell(0, 2, 4), new Cell(0, 3, 16) };
            List<int> ExpectedValues = new List<int> { 2, 8, 4, 16 };

            GetGrid(ref grid, SetCells);

            List<Move> Moves;
            List<PositionOnBoard> ValuesChanged;
            int scoreDelta = 0;
            Moves = grid.MoveAllCells(DragDirection.Left, out ValuesChanged, ref scoreDelta);

            for (int column = 0; column < 4; column++)
            {
                Assert.AreEqual(ExpectedValues[column], grid.Cells[0, column].Value);
            }
        }

        [TestMethod]
        [TestCategory("Game Unit Testing")]
        public void TestGameOver()
        {
            Game game = new Game(new MainWindow());
            PlayingGrid grid = new PlayingGrid();
            List<Cell> SetCells = GetFullGridOfCells();
            GetGrid(ref grid, SetCells);
            game.Grid = grid;

            bool gameOver;
            int scoreDelta;
            game.MakeMove(DragDirection.Left, out scoreDelta, out gameOver);

            Assert.IsTrue(gameOver);
        }

        [TestMethod]
        [TestCategory("Game Unit Testing")]
        public void TestRestartAfterGameOver()
        {
            MainWindow Window = new MainWindow();
            Game game = new Game(Window);
            PlayingGrid grid = new PlayingGrid();
            List<Cell> SetCells = GetFullGridOfCells();
            GetGrid(ref grid, SetCells);
            Window.game.Grid = grid;

            bool gameOver;
            int scoreDelta;
            Window.game.MakeMove(DragDirection.Left, out scoreDelta, out gameOver);

            Window.game.Parent.OnRestartPressed();
            Assert.IsTrue(App.GameRunning);

            Window.game.MakeMove(DragDirection.Left, out scoreDelta, out gameOver);

            Assert.IsFalse(gameOver);

            int emptyCells = Window.game.Grid.CountEmptyCells();
            Assert.AreEqual((scoreDelta == 0 ? 13 : 14), emptyCells);
        }

        private List<Cell> GetFullGridOfCells()
        {
            List<Cell> Cells = new List<Cell>
            {
                new Cell(0,0,16),
                new Cell(0,1,8),
                new Cell(0,2,2),
                new Cell(0,3,2),
                new Cell(1,0,32),
                new Cell(1,1,16),
                new Cell(1,2,8),
                new Cell(1,3,4),
                new Cell(2,0,64),
                new Cell(2,1,32),
                new Cell(2,2,16),
                new Cell(2,3,8),
                new Cell(3,0,128),
                new Cell(3,1,64),
                new Cell(3,2,32),
                new Cell(3,3,16),
            };

            return Cells;
        }

        public Game StartNewGame()
        {
            MainWindow window = new MainWindow();
            Game game = new Game(window);

            return game;
        }

        public void GetGrid(ref PlayingGrid grid, List<Cell> Cells)
        {
            foreach (Cell Cell in Cells)
            {
                grid.Cells[Cell.Row, Cell.Column].Value = Cell.Value;
            }

            grid.SetCanSizeInDirection();
        }
    }
}
