using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using V22048Game.Elements;
using V22048Game.MoveInformation;

namespace V22048Test
{
    [TestClass]
    public class MoveLogicTesting
    {
        [TestMethod]
        [TestCategory("MoveTesting")]
        [TestProperty("Direction", "Left")]
        [TestProperty("GridLayout", "2,2,0,2")]
        public void TestMergeAndMoveLeft()
        {
            Board testBoard = InitialiseBoard(new int[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 0, 2});
            
            int score, highBlock;
            Board nextBoard = testBoard.TryMove(Direction.Left, out score, out highBlock);

            Assert.AreEqual(4, score);
            Assert.AreEqual(4, highBlock);
            Assert.AreEqual(4, nextBoard[3, 0].Value.Mod);
            Assert.AreEqual(2, nextBoard[3, 1].Value.Mod);
            Assert.AreEqual(0, nextBoard[3, 2].Value.Mod);
        }

        [TestMethod]
        [TestCategory("MoveTesting")]
        [TestProperty("Direction", "Up")]
        [TestProperty("GridLayout", "2,2,0,2")]
        public void TestMergeAndMoveUp1()
        {
            Board testBoard = InitialiseBoard(new int[] {2,0,0,0,2,0,0,0,0,0,0,0,2,0,0,0 });

            int score, highBlock;
            Board nextBoard = testBoard.TryMove(Direction.Up, out score, out highBlock);

            Assert.AreEqual(4, score);
            Assert.AreEqual(4, highBlock);
            Assert.AreEqual(4, nextBoard[0, 0].Value.Mod);
            Assert.AreEqual(2, nextBoard[1, 0].Value.Mod);
            Assert.AreEqual(0, nextBoard[2, 0].Value.Mod);
        }

        [TestMethod]
        [TestCategory("MoveTesting")]
        [TestProperty("Direction", "Up")]
        [TestProperty("GridLayout", "8,2,0,2")]
        public void TestMergeAndMoveUp2()
        {
            Board testBoard = InitialiseBoard(new int[] { 8, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 2, 0, 0, 0 });

            int score, highBlock;
            Board nextBoard = testBoard.TryMove(Direction.Up, out score, out highBlock);

            Assert.AreEqual(4, score);
            Assert.AreEqual(4, highBlock);
            Assert.AreEqual(8, nextBoard[0, 0].Value.Mod);
            Assert.AreEqual(4, nextBoard[1, 0].Value.Mod);
            Assert.AreEqual(0, nextBoard[2, 0].Value.Mod);
        }

        [TestMethod]
        [TestCategory("MoveTesting")]
        [TestProperty("Direction", "Down")]
        [TestProperty("GridLayout", "2,2,2,4")]
        public void TestMergeAndMoveDown1()
        {
            Board testBoard = InitialiseBoard(new int[] { 2, 0, 0, 0, 2, 0, 0, 0, 2, 0, 0, 0, 4, 0, 0, 0 });

            int score, highBlock;
            Board nextBoard = testBoard.TryMove(Direction.Down, out score, out highBlock);

            Assert.AreEqual(4, score);
            Assert.AreEqual(4, highBlock);
            Assert.AreEqual(4, nextBoard[3, 0].Value.Mod);
            Assert.AreEqual(4, nextBoard[2, 0].Value.Mod);
            Assert.AreEqual(2, nextBoard[1, 0].Value.Mod);
            Assert.AreEqual(0, nextBoard[0, 0].Value.Mod);
        }

        [TestMethod]
        [TestCategory("BoardAnalysisTesting")]
        [TestProperty("Direction", "Left")]
        [TestProperty("GridLayout", "2,2,0,2")]
        public void TestAnalyseGridNextMoves()
        {
            Board testBoard = InitialiseBoard(new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 0, 2 });

            Assert.AreEqual(0, testBoard[3, 0].NextPosition[Direction.Left]);
            Assert.AreEqual(0, testBoard[3, 1].NextPosition[Direction.Left]);
            Assert.AreEqual(1, testBoard[3, 3].NextPosition[Direction.Left]);
        }

        [TestMethod]
        [TestCategory("BoardAnalysisTesting")]
        [TestProperty("Direction", "Left")]
        [TestProperty("GridLayout", "2,2,0,2")]
        public void TestAnalyseGridMoveActions1()
        {
            Board testBoard = InitialiseBoard(new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 2, 0, 2 });

            Assert.AreEqual(MoveAction.Zero, testBoard[3, 0].NextMoveAction[Direction.Left]);
            Assert.AreEqual(MoveAction.Act, testBoard[3, 1].NextMoveAction[Direction.Left]);
            Assert.AreEqual(MoveAction.Constant, testBoard[3, 3].NextMoveAction[Direction.Left]);
        }
        [TestMethod]
        [TestCategory("BoardAnalysisTesting")]
        [TestProperty("Direction", "Left")]
        [TestProperty("GridLayout", "2,0,2,2")]
        public void TestAnalyseGridMoveActions2()
        {
            Board testBoard = InitialiseBoard(new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2, 0,2, 2 });

            Assert.AreEqual(MoveAction.Zero, testBoard[3, 0].NextMoveAction[Direction.Left]);
            Assert.AreEqual(MoveAction.Act, testBoard[3, 2].NextMoveAction[Direction.Left]);
            Assert.AreEqual(MoveAction.Constant, testBoard[3, 3].NextMoveAction[Direction.Left]);
        }
        [TestMethod]
        [TestCategory("BoardAnalysisTesting")]
        [TestProperty("Direction", "Left")]
        [TestProperty("GridLayout", "0,2,2,2")]
        public void TestAnalyseGridMoveActions3()
        {
            Board testBoard = InitialiseBoard(new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,0,2, 2, 2 });

            Assert.AreEqual(MoveAction.Zero, testBoard[3, 1].NextMoveAction[Direction.Left]);
            Assert.AreEqual(MoveAction.Act, testBoard[3, 2].NextMoveAction[Direction.Left]);
            Assert.AreEqual(MoveAction.Constant, testBoard[3, 3].NextMoveAction[Direction.Left]);
        }

        private Board InitialiseBoard(int[] cellValues)
        {
            Board board = new Board(4);
            board.Cells = GetCells(cellValues);
            board.AnalyseNextMoveDistances();

            return board;
        }

        private Cell[,] GetCells(int[] cellValues)
        {
            Cell[,] cells = new Cell[4, 4];
            GameValue cellValue;
            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    cellValue = new GameValue(cellValues[row * 4 + column], 0);
                    cells[row, column] = new Cell(row, column, cellValue, (int)Math.Log(cellValues[row * 4 + column], 2) + 4);
                }
            }

            return cells;
        }
    }
}
