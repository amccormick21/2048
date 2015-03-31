using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _2048Game;
using _2048Game.AI;
using _2048Game.Board;
using _2048Game.UI;
using _2048Game.GamePlay;

namespace _2048Test
{
    [TestClass]
    public class AITesting
    {
        public static DragDirection MoveResult;

        [TestClass]
        public class GetMoveTesting
        {
            [TestMethod]
            [TestCategory("Generic Move Testing")]
            public void Test1()
            {
                PlayingGrid Grid = AITesting.GetGrid(4, 8, 32, 64, 0, 2, 0, 2, 2, 4, 2, 2, 0, 2, 4, 2);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);
                /*4   0   2   0   
                  8   2   4   2
                  32  0   2   4
                  64  2   2   2*/
                DragDirection move = AI.GetMoveToMake();

                Assert.AreEqual(DragDirection.Left, move);
            }

            [TestMethod]
            [TestCategory("Generic Move Testing")]
            public void TestChainCompletion()
            {
                App.GameRunning = true;
                MainWindow Window = new MainWindow();
                Game Game = new Game(Window);
                PlayingGrid Grid = AITesting.GetGrid(16, 32, 64, 128, 8, 4, 2, 2, 0, 0, 0, 0, 2, 0, 0, 0);

                int iterations = 0;

                Window.AnimationsComplete += (s, e) =>
                {
                    if (++iterations == 7)
                    {
                        Game.AIPlay = false;
                        Assert.AreEqual(256, Game.Grid[3, 0]);
                    }
                };

                Game.Grid = Grid;
                Game.AIPlay = true;
            }
        }

        [TestClass]
        public class FillBottomLeftTesting
        {
            [TestMethod]
            [TestCategory("Fill Bottom Left")]
            public void Test1()
            {
                PlayingGrid Grid = AITesting.GetGrid(2, 4, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                DragDirection move = AI.TryFillBottomLeft();

                Assert.AreEqual(DragDirection.Down, move);
            }

            [TestMethod]
            [TestCategory("Fill Bottom Left")]
            public void Test2()
            {
                PlayingGrid Grid = AITesting.GetGrid(2, 4, 8, 0, 2, 4, 4, 16, 8, 2, 2, 8, 2, 0, 0, 4);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                DragDirection move = AI.TryFillBottomLeft();

                Assert.AreEqual(DragDirection.Left, move);
            }

            [TestMethod]
            [TestCategory("Fill Bottom Left")]

            public void Test3()
            {
                PlayingGrid Grid = AITesting.GetGrid(2, 4, 2, 0, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                DragDirection move = AI.TryFillBottomLeft();

                Assert.AreEqual(DragDirection.Down, move);
            }

            [TestMethod]
            [TestCategory("Fill Bottom Left")]

            public void Test4()
            {
                PlayingGrid Grid = AITesting.GetGrid(0, 0, 0, 0, 0, 0, 2, 0, 0, 2, 0, 0, 0, 0, 0, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                DragDirection move = AI.TryFillBottomLeft();

                Assert.AreEqual(DragDirection.Left, move);
            }
        }

        [TestClass]
        public class GetMoveFromChainTesting
        {
            [TestMethod]
            [TestCategory("Move From Chain")]
            public void Test1()
            {
                PlayingGrid Grid = AITesting.GetGrid(8, 16, 32, 64, 4, 2, 4, 0, 2, 4, 2, 0, 8, 2, 4, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                DragDirection move = AI.GetMoveFromChain();

                Assert.AreEqual(DragDirection.Down, move);
            }

            [TestMethod]
            [TestCategory("Move From Chain")]
            public void Test2()
            {
                PlayingGrid Grid = AITesting.GetGrid(16, 16, 32, 64, 8, 4, 2, 0, 0, 0, 0, 0, 0, 2, 2, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                DragDirection move = AI.GetMoveFromChain();

                Assert.AreEqual(DragDirection.Down, move);
            }

            [TestMethod]
            [TestCategory("Move From Chain")]
            public void Test3()
            {
                PlayingGrid Grid = AITesting.GetGrid(0, 16, 32, 64, 0, 8, 8, 0, 0, 2, 4, 0, 0, 4, 2, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                DragDirection move = AI.GetMoveFromChain();

                Assert.AreNotEqual(DragDirection.Up, move);
            }
        }

        [TestClass]
        public class CellSurroundedTesting
        {
            [TestMethod]
            [TestCategory("Cell Surrounded")]
            public void Test1()
            {
                PlayingGrid Grid = AITesting.GetGrid(4, 8, 16, 32, 8, 4, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                DragDirection move = AI.GetMoveFromChain();

                Assert.AreEqual(DragDirection.Down, move);
            }


            [TestMethod]
            [TestCategory("Cell Surrounded")]
            public void Test2()
            {
                PlayingGrid Grid = AITesting.GetGrid(8, 16, 32, 64, 4, 4, 2, 4, 2, 0, 0, 0, 2, 0, 0, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                DragDirection move = AI.GetMoveFromChain();

                Assert.AreEqual(DragDirection.Up, move);
            }

            /// <summary>
            /// New test to check that the routine puts an equal cell next to the target cell
            /// </summary>
            [TestMethod]
            [TestCategory("Cell Surrounded")]
            public void Test3()
            {
                PlayingGrid Grid = AITesting.GetGrid(32, 16, 32, 64, 16, 32, 8, 0, 0, 0, 0, 0, 0, 2, 0, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                DragDirection move = AI.GetMoveFromChain();

                Assert.AreEqual(DragDirection.Down, move);
            }

            /// <summary>
            /// Tests the recursion in the chain routine
            /// </summary>
            [TestMethod]
            [TestCategory("Cell Surrounded")]
            public void Test4()
            {
                PlayingGrid Grid = AITesting.GetGrid(8, 16, 64, 128, 8, 2, 8, 0, 2, 4, 4, 2, 0, 2, 4, 2);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                DragDirection move = AI.GetMoveFromChain();

                Assert.AreEqual(DragDirection.Left, move);
            }

            /// <summary>
            /// Tests that the system actively tries to move other cells away from an isolated cell.
            /// </summary>
            /// <remarks>The routine should identify that [0,1] is surrounded and needs to be cleared,
            /// by moving down to set up a chain off [0,2].</remarks>
            [TestCategory("Secondary testing")]
            [TestCategory("Cell Surrounded")]
            [TestMethod]
            public void Test5()
            {
                PlayingGrid Grid = AITesting.GetGrid(8, 16, 64, 128, 2, 8, 4, 2, 16, 8, 4, 2, 2, 4, 2, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                DragDirection move = AI.GetMoveFromChain();

                Assert.AreEqual(DragDirection.Down, move);
            }
        }
        [TestClass]
        public class CellNotSurroundedTesting
        {
            [TestMethod]
            [TestCategory("Cell Not Surrounded")]
            public void Test1()
            {
                PlayingGrid Grid = AITesting.GetGrid(8, 16, 32, 64, 0, 0, 2, 0, 8, 0, 0, 2, 0, 0, 0, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                DragDirection move = AI.GetMoveFromChain();

                Assert.AreEqual(DragDirection.Left, move);
            }

            [TestMethod]
            [TestCategory("Cell Not Surrounded")]
            public void Test2()
            {
                PlayingGrid Grid = AITesting.GetGrid(8, 16, 32, 64, 0, 8, 2, 0, 0, 0, 2, 0, 0, 4, 2, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                DragDirection move = AI.GetMoveFromChain();

                Assert.AreEqual(DragDirection.Up, move);
            }

            /// <summary>
            /// Tests that the routine puts the highest possible cell next to the target
            /// </summary>
            [TestMethod]
            [TestCategory("Cell Not Surrounded")]
            public void Test3()
            {
                PlayingGrid Grid = AITesting.GetGrid(8, 16, 32, 64, 0, 4, 2, 0, 2, 2, 0, 4, 0, 0, 2, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                DragDirection move = AI.GetMoveFromChain();

                Assert.AreEqual(DragDirection.Up, move);
            }
        }

        [TestClass]
        public class LeastDamagingMoveTesting
        {
            /// <summary>
            /// Tests that the routine finds the only possible move
            /// </summary>
            [TestMethod]
            [TestCategory("Least Damaging Move")]
            public void Test1()
            {
                PlayingGrid Grid = AITesting.GetGrid(4, 8, 16, 32, 8, 4, 2, 8, 0, 0, 0, 2, 0, 0, 0, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                DragDirection move = AI.GetMoveFromChain();

                Assert.AreEqual(DragDirection.Up, move);
            }

            /// <summary>
            /// Tests that the routine finds the only possible allowable move
            /// </summary>
            [TestMethod]
            [TestCategory("Least Damaging Move")]
            public void Test2()
            {
                PlayingGrid Grid = AITesting.GetGrid(4, 8, 16, 32, 8, 4, 2, 8, 0, 2, 0, 0, 0, 0, 0, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                DragDirection move = AI.GetMoveFromChain();

                Assert.AreEqual(DragDirection.Up, move);
            }

            /// <summary>
            /// Tests that the routine moves in the first direction in chain when required
            /// </summary>
            [TestMethod]
            [TestCategory("Least Damaging Move")]
            public void Test3()
            {
                PlayingGrid Grid = AITesting.GetGrid(0, 8, 16, 32, 0, 32, 8, 4, 0, 2, 4, 2, 0, 0, 0, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                DragDirection move = AI.GetMoveFromChain();

                Assert.AreEqual(DragDirection.Up, move);
            }
        }

        [TestClass]
        public class LargestCellOnGridTesting
        {
            /// <summary>
            /// Tests that the routine finds the largest cell on the grid.
            /// </summary>
            [TestMethod]
            [TestCategory("Function Testing")]
            public void Test1()
            {
                PlayingGrid Grid = AITesting.GetGrid(0, 8, 16, 32, 16, 32, 8, 4, 4, 2, 4, 2, 0, 0, 0, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                Cell cell = Grid.GetLargestCellOnGrid();

                Assert.IsTrue(cell.Equals(new Cell(3, 0, 32)));
            }

            /// <summary>
            /// Tests that the routine finds the largest cell on the grid.
            /// </summary>
            [TestMethod]
            [TestCategory("Function Testing")]
            public void Test2()
            {
                PlayingGrid Grid = AITesting.GetGrid(0, 2, 4, 2, 16, 32, 64, 0, 2, 4, 2, 0, 0, 4, 2, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                Cell cell = Grid.GetLargestCellOnGrid();

                Assert.IsTrue(cell.Equals(new Cell(2, 1, 64)));
            }
        }
        [TestClass]
        public class IsMoveDangerousTesting
        {
            [TestCategory("Is Move Dangerous")]
            [TestMethod]
            public void Test1()
            {
                PlayingGrid Grid = AITesting.GetGrid(4, 8, 16, 32, 0, 0, 4, 8, 0, 2, 0, 0, 0, 0, 0, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                bool isDangerous = AI.IsMoveDangerous(DragDirection.Left);

                Assert.IsTrue(isDangerous);
            }

            [TestCategory("Is Move Dangerous")]
            [TestMethod]
            public void Test2()
            {
                PlayingGrid Grid = AITesting.GetGrid(4, 8, 16, 32, 0, 2, 4, 8, 0, 2, 0, 0, 0, 0, 0, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                bool isDangerous = AI.IsMoveDangerous(DragDirection.Left);

                Assert.IsFalse(isDangerous);
            }

            [TestCategory("Is Move Dangerous")]
            [TestMethod]
            public void Test3()
            {
                PlayingGrid Grid = AITesting.GetGrid(4, 8, 16, 32, 2, 2, 8, 16, 0, 0, 0, 0, 0, 0, 0, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);

                bool isDangerous = AI.IsMoveDangerous(DragDirection.Down);

                Assert.IsTrue(isDangerous);
            }

        }
        [TestClass]
        public class IsCellSurroundedTesting
        {
            [TestCategory("Is Cell Surrounded")]
            [TestMethod]
            public void Test1()
            {
                PlayingGrid Grid = AITesting.GetGrid(4, 8, 16, 32, 8, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);
                var Cell = Grid.Cells[0, 0];

                bool isEqualTo;
                bool allHigherThan;
                int highestLessThan;
                bool isSurrounded = Cell.IsSurrounded(out isEqualTo, out allHigherThan, out highestLessThan, Grid);

                Assert.IsTrue(isSurrounded);
                Assert.IsFalse(isEqualTo);
                Assert.IsTrue(allHigherThan);
                Assert.AreEqual(0, highestLessThan);
            }

            [TestCategory("Is Cell Surrounded")]
            [TestMethod]
            public void Test2()
            {
                PlayingGrid Grid = AITesting.GetGrid(4, 8, 16, 32, 0, 0, 2, 0, 0, 0, 0, 0, 2, 0, 0, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);
                var Cell = Grid.Cells[0, 0];

                bool isEqualTo;
                bool allHigherThan;
                int highestLessThan;
                bool isSurrounded = Cell.IsSurrounded(out isEqualTo, out allHigherThan, out highestLessThan, Grid);

                Assert.IsFalse(isSurrounded);
                Assert.IsFalse(isEqualTo);
                Assert.IsFalse(allHigherThan);
                Assert.AreEqual(0, highestLessThan);
            }

            [TestCategory("Is Cell Surrounded")]
            [TestMethod]
            public void Test3()
            {
                PlayingGrid Grid = AITesting.GetGrid(4, 2, 0, 0, 2, 2, 4, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);
                var Cell = Grid.Cells[0, 0];

                bool isEqualTo;
                bool allHigherThan;
                int highestLessThan;
                bool isSurrounded = Cell.IsSurrounded(out isEqualTo, out allHigherThan, out highestLessThan, Grid);

                Assert.IsTrue(isSurrounded);
                Assert.IsFalse(isEqualTo);
                Assert.IsFalse(allHigherThan);
                Assert.AreEqual(2, highestLessThan);
            }

            [TestCategory("Is Cell Surrounded")]
            [TestMethod]
            public void Test4()
            {
                PlayingGrid Grid = AITesting.GetGrid(4, 2, 0, 0, 0, 0, 0, 2, 2, 0, 0, 0, 0, 0, 0, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);
                var Cell = Grid.Cells[0, 0];

                bool isEqualTo;
                bool allHigherThan;
                int highestLessThan;
                bool isSurrounded = Cell.IsSurrounded(out isEqualTo, out allHigherThan, out highestLessThan, Grid);

                Assert.IsFalse(isSurrounded);
                Assert.IsFalse(isEqualTo);
                Assert.IsFalse(allHigherThan);
                Assert.AreEqual(2, highestLessThan);
            }

            [TestCategory("Is Cell Surrounded")]
            [TestMethod]
            public void Test5()
            {
                PlayingGrid Grid = AITesting.GetGrid(4, 4, 0, 2, 2, 2, 0, 0, 4, 0, 0, 0, 0, 0, 0, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);
                var Cell = Grid.Cells[0, 0];

                bool isEqualTo;
                bool allHigherThan;
                int highestLessThan;
                bool isSurrounded = Cell.IsSurrounded(out isEqualTo, out allHigherThan, out highestLessThan, Grid);

                Assert.IsTrue(isSurrounded);
                Assert.IsTrue(isEqualTo);
                Assert.IsFalse(allHigherThan);
                Assert.AreEqual(2, highestLessThan);
            }

            /// <summary>
            /// Extra test to make sure the routine works for a cell in the centre of the board
            /// </summary>
            [TestCategory("Is Cell Surrounded")]
            [TestMethod]
            public void Test6()
            {
                PlayingGrid Grid = AITesting.GetGrid(0, 4, 0, 0, 8, 8, 16, 0, 0, 2, 0, 0, 0, 0, 0, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);
                var Cell = Grid.Cells[1, 1];

                bool isEqualTo;
                bool allHigherThan;
                int highestLessThan;
                bool isSurrounded = Cell.IsSurrounded(out isEqualTo, out allHigherThan, out highestLessThan, Grid);

                Assert.IsTrue(isSurrounded);
                Assert.IsTrue(isEqualTo);
                Assert.IsFalse(allHigherThan);
                Assert.AreEqual(4, highestLessThan);
            }
        }

        [TestClass]
        public class GetCellInDirectionTesting
        {
            [TestCategory("Get Cell In Direction")]
            [TestMethod]
            public void Test1()
            {
                PlayingGrid Grid = AITesting.GetGrid(4, 2, 0, 2, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);
                var Cell = Grid.Cells[0, 0];

                Cell cellFound;
                bool cellExists = Grid.GetCellInDirection(Cell, DragDirection.Left, out cellFound);

                Assert.IsFalse(cellExists);
            }

            [TestCategory("Get Cell In Direction")]
            [TestMethod]
            public void Test2()
            {
                PlayingGrid Grid = AITesting.GetGrid(0, 0, 4, 0, 2, 0, 2, 0, 0, 4, 2, 0, 0, 8, 2, 0);
                ArtificialIntelligence AI = AITesting.SetupAI(Grid);
                var Cell = Grid.Cells[2, 2];

                Cell cellFound;
                bool cellExists = Grid.GetCellInDirection(Cell, DragDirection.Down, out cellFound);

                Assert.IsTrue(cellExists);
                Assert.AreEqual(0, cellFound.Value);
            }
        }

        public static PlayingGrid GetGrid(
            int r0c0, int r1c0, int r2c0, int r3c0,
            int r0c1, int r1c1, int r2c1, int r3c1,
            int r0c2, int r1c2, int r2c2, int r3c2,
            int r0c3, int r1c3, int r2c3, int r3c3
            )
        {
            PlayingGrid Grid = new PlayingGrid();
            Grid[0, 0] = r0c0;
            Grid[1, 0] = r1c0;
            Grid[2, 0] = r2c0;
            Grid[3, 0] = r3c0;
            Grid[0, 1] = r0c1;
            Grid[1, 1] = r1c1;
            Grid[2, 1] = r2c1;
            Grid[3, 1] = r3c1;
            Grid[0, 2] = r0c2;
            Grid[1, 2] = r1c2;
            Grid[2, 2] = r2c2;
            Grid[3, 2] = r3c2;
            Grid[0, 3] = r0c3;
            Grid[1, 3] = r1c3;
            Grid[2, 3] = r2c3;
            Grid[3, 3] = r3c3;

            Grid.SetCanSizeInDirection();

            return Grid;
        }

        public static ArtificialIntelligence SetupAI(PlayingGrid Grid)
        {
            ArtificialIntelligence AI = new ArtificialIntelligence(Grid);
            AI.Move += (Direction) => { MoveResult = Direction; };
            
            return AI;
        }
    }
}
