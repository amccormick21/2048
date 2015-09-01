using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2048Game.Board;
using _2048Game.GamePlay;

namespace _2048Game.AI
{
    /* TODO:
     * Get tests to pass - some things are still not working perfectly
     * Identify when if a column is filled, a chain can be completed
     * Identify when a move can provide a 'stepping stone' to complete a chain
     * Incorporate tests... somehow
     */

    public class ArtificialIntelligence
    {
        PlayingGrid grid;
        Game game;
        bool generateMoves;

        DragDirection nextMove;

        /// <summary>
        /// Creates a new instance of the AI class for the 2048 game.
        /// </summary>
        /// <param name="Grid">The current status of the board</param>
        /// <param name="Game">The game to which the AI will be linked</param>
        public ArtificialIntelligence(PlayingGrid Grid, Game Game)
        {
            this.grid = Grid;
            this.game = Game;
            generateMoves = true;
            Move += Game.AI_Move;
            TreeFound += Game.TreeFound;
            Game.GameOver += Game_GameOver;
        }
        public ArtificialIntelligence(PlayingGrid Grid)
        {
            this.Grid = Grid;
            generateMoves = true;
        }

        void Game_GameOver(GameOverEventArgs e)
        {
            generateMoves = false;
        }

        public void FireNextMove()
        {
            if (this.Grid.CanMove[nextMove])
            {
                if (Move != null)
                    Move(nextMove);
            }
        }

        /// <summary>
        /// Determines the best move to make and fires the Move event with this direction.
        /// </summary>
        public void PlanNextMove()
        {
            if (generateMoves)
            {
                if (!this.Grid.IsGameOver())
                {
                    try
                    {
                        nextMove = GetMoveToMake();
                    }
                    catch (InvalidOperationException ex)
                    {
                        Console.Out.WriteLine(ex.Message);
                    }
                }
            }
        }

        public DragDirection GetMoveToMake()
        {
            DragDirection MoveToMake;
            if (Grid[3, 0] == 0)
            {
                MoveToMake = TryFillBottomLeft();
            }
            else
            {
                //MoveToMake = GetMoveFromChain(this.Grid);
                MoveToMake = GetDepthSearchForMove();
            }

            if (this.grid.CanMove[MoveToMake])
            {
                return MoveToMake;
            }
            else
            {
                throw new InvalidOperationException("Move cannot be made on grid");
            }
        }

        private DragDirection GetDepthSearchForMove()
        {
            //TODO: make recursive and set time limit based on AI speed
            DragDirection directionHolder = DragDirection.Left;
            int highestFirstDirectionHolder = -1;
            int highestSecondDirectionHolder = 0;
            int highestCellImprovedFirstDepth, highestCellImprovedSecondDepth;
            PlayingGrid newGrid;
            foreach (DragDirection direction in DragDirections.AllDragDirections)
            {
                if (this.Grid.CanMove[direction] && !IsMoveDangerous(direction) && this.Grid.Cells[3, 0].MaxScaleInDirection[direction] == 0)
                {
                    newGrid = TryMove(direction, this.Grid);
                    highestCellImprovedFirstDepth = GetHighestCellImproved(this.Grid, newGrid);

                    foreach (DragDirection secondDirection in DragDirections.AllDragDirections)
                    {
                        if (newGrid.CanMove[direction] && newGrid.Cells[3, 0].MaxScaleInDirection[direction] == 0)
                        {
                            highestCellImprovedSecondDepth = GetHighestCellImproved(newGrid, TryMove(secondDirection, newGrid));
                            if (highestSecondDirectionHolder < highestCellImprovedSecondDepth)
                            {
                                highestSecondDirectionHolder = highestCellImprovedSecondDepth;
                            }
                        }
                    }
                    highestCellImprovedFirstDepth += highestSecondDirectionHolder;
                    if (highestCellImprovedFirstDepth > highestFirstDirectionHolder)
                    {
                        highestFirstDirectionHolder = highestCellImprovedFirstDepth;
                        directionHolder = direction;
                    }
                }
            }

            if (highestFirstDirectionHolder == -1)
            {
                //Try to find any move with the greatest score delta
                int highestScoreDelta = -1;
                int scoreDelta;
                foreach (DragDirection direction in DragDirections.AllDragDirections)
                {
                    if (this.Grid.CanMove[direction])
                    {
                        newGrid = TryMove(direction, Grid, out scoreDelta);
                        if (scoreDelta > highestScoreDelta)
                        {
                            scoreDelta = highestScoreDelta;
                            directionHolder = direction;
                        }
                    }
                }

            }

            return directionHolder;
        }

        public delegate void MoveEventHandler(DragDirection Direction);
        public event MoveEventHandler Move;

        public PlayingGrid Grid
        {
            get { return grid; }
            private set { grid = value; }
        }

        public DragDirection TryFillBottomLeft()
        {
            Cell largestCell = Grid.GetLargestCellOnGrid();
            if (largestCell.Column - largestCell.MaxScaleInDirection[DragDirection.Left] == 0
                && largestCell.Row == 3)
            {
                return DragDirection.Left;
            }
            else
            {
                if (largestCell.Row + largestCell.MaxScaleInDirection[DragDirection.Down] == 3
                    && largestCell.Column == 0)
                {
                    return DragDirection.Down;
                }
                else
                {
                    PlayingGrid testGrid = TryMove(DragDirection.Down, this.Grid);
                    if (testGrid[3, 0] != 0)
                    {
                        return DragDirection.Down;
                    }
                    else
                    {
                        return DragDirection.Left;
                    }
                }
            }
        }

        private PlayingGrid TryMove(DragDirection dragDirection, PlayingGrid Grid)
        {
            int scoreDelta;
            return TryMove(dragDirection, Grid, out scoreDelta);
        }

        private PlayingGrid TryMove(DragDirection dragDirection, PlayingGrid Grid, out int scoreDelta)
        {
            PlayingGrid gridCopy = new PlayingGrid(Grid);
            scoreDelta = 0;
            List<PositionOnBoard> valuesChanged;
            gridCopy.MoveAllCells(dragDirection, out valuesChanged, ref scoreDelta);
            gridCopy.SetCanSizeInDirection();
            return gridCopy;
        }

        /// <summary>
        /// Compares trees on grids before and after moves to asses which move is best.
        /// </summary>
        /// <returns>The direction of the move which helps the grid the most while disturbing it the least</returns>
        public DragDirection GetMoveFromChain(PlayingGrid Grid)
        {
            //Setup parameters
            CellTree tree = GetCellTree(Grid);
            if (TreeFound != null)
                TreeFound(this, tree);

            PlayingGrid trialGridAfterMoveInDirection;
            DragDirection bestDirectionHolder = DragDirection.Left;

            //Set the holders to intial values
            int highestCellImprovedHolder = 0;

            //Try a move in all directions:
            foreach (DragDirection direction in DragDirections.AllDragDirections)
            {
                //If a move can be made that is not dangerous, and does not disturb the bottom corner.
                if (Grid.CanMove[direction] && !IsMoveDangerous(direction) && Grid.Cells[3, 0].MaxScaleInDirection[direction] == 0)
                {
                    //Try a move in this direction
                    trialGridAfterMoveInDirection = TryMove(direction, Grid);

                    int highestCellImproved = GetHighestCellImproved(Grid, trialGridAfterMoveInDirection);
                    int highestCellMoved = tree.HighestCellMovedInDirection[direction];

                    if (highestCellMoved < highestCellImproved)
                    {
                        if (highestCellImproved > highestCellImprovedHolder)
                        {
                            highestCellImprovedHolder = highestCellImproved;
                            bestDirectionHolder = direction;
                        }
                    }

                }
            }

            //Try to find any move with the greatest score delta
            int highestScoreDelta = -1;
            int scoreDelta;
            if (highestCellImprovedHolder == 0)
            {
                foreach (DragDirection direction in DragDirections.AllDragDirections)
                {
                    if (Grid.CanMove[direction])
                    {
                        trialGridAfterMoveInDirection = TryMove(direction, Grid, out scoreDelta);
                        if (scoreDelta > highestScoreDelta)
                        {
                            scoreDelta = highestScoreDelta;
                            bestDirectionHolder = direction;
                        }
                    }
                }
            }

            return bestDirectionHolder;
        }

        /// <summary>
        /// Gets the value of the highest cell that has been benefitted by the change in grids.
        /// </summary>
        /// <param name="oldGrid">The grid before a move</param>
        /// <param name="newGrid">The grid after a move</param>
        /// <returns>The value of the highest cell benefitted.</returns>
        private int GetHighestCellImproved(PlayingGrid oldGrid, PlayingGrid newGrid)
        {
            int highestCellImprovedHolder = 0;

            //Compare the trees for:
            //  -   merged cells;
            //  -   completed roots;
            //  -   new cells added
            CellTree oldTree = GetCellTree(oldGrid);
            CellTree newTree = GetCellTree(newGrid);
            int highestCellImprovedFromComparedTrees = GetHighestCellImprovedFromCompareTrees(oldTree, newTree);
            SwapIfHigher(ref highestCellImprovedHolder, highestCellImprovedFromComparedTrees);

            return highestCellImprovedHolder;
        }

        private int GetHighestCellImprovedFromCompareTrees(CellTree oldTree, CellTree newTree)
        {
            int highestCellImprovedHolder = 0;

            int longestTree;
            if (oldTree.NodesInChain.Count > newTree.NodesInChain.Count)
            { longestTree = oldTree.NodesInChain.Count; }
            else
            { longestTree = newTree.NodesInChain.Count; }

            //cycle through pointers to find the first cell that has been doubled.
            int highestDoubledCell = GetHighestDoubledCell(oldTree, newTree, oldTree.LastNode);
            SwapIfHigher(ref highestCellImprovedHolder, highestDoubledCell);

            //now try to get any cell that has been added and completes a root:
            int highestCompleteRootCell = GetHighestCompleteRootCell(oldTree, newTree, oldTree.LastNode);
            SwapIfHigher(ref highestCellImprovedHolder, highestCompleteRootCell);

            //try and add a cell to the tree
            int highestCellAddedToTree = GetHighestCellAddedToTree(oldTree, newTree);
            SwapIfHigher(ref highestCellImprovedHolder, highestCellAddedToTree);

            return highestCellImprovedHolder;
        }

        private int GetHighestCellAddedToTree(CellTree oldTree, CellTree newTree)
        {
            int highestCellAddedHolder = 0;
            foreach (TreeNode node in oldTree.NodesInChain)
            {
                TreeNode mirrorOnNewTree = newTree.NodesInChain.Find(t => t.Position.Equals(node.Position));

                if (!mirrorOnNewTree.Equals(new TreeNode()))
                {
                    foreach (DragDirection direction in DragDirections.AllDragDirections)
                    {
                        if (node.Pointer[direction].Equals(new TreeNode()) && !mirrorOnNewTree.Pointer[direction].Equals(new TreeNode()))
                        {
                            SwapIfHigher(ref highestCellAddedHolder, mirrorOnNewTree.Pointer[direction].Value);
                        }
                    }
                }
            }

            return highestCellAddedHolder;
        }

        private int GetHighestCompleteRootCell(CellTree oldTree, CellTree newTree, TreeNode nodeToCheck)
        {
            int highestCompleteRootCell = 0;
            int thisHighestRootCell;

            if (!nodeToCheck.Equals(new TreeNode()))
            {
                //Find the node in new tree at the same position.
                TreeNode mirrorOnNewTree = newTree.NodesInChain.Find(t => t.Position.Equals(nodeToCheck.Position));

                if (!mirrorOnNewTree.Equals(new TreeNode()))
                {
                    foreach (DragDirection direction in DragDirections.AllDragDirections)
                    {
                        if (!nodeToCheck.Pointer[direction].Equals(new TreeNode()))
                        {
                            if (mirrorOnNewTree.Pointer[direction].Value == mirrorOnNewTree.Value)
                            {
                                SwapIfHigher(ref highestCompleteRootCell, mirrorOnNewTree.Value);
                            }
                        }
                    }
                }

                foreach (DragDirection direction in DragDirections.AllDragDirections)
                {
                    thisHighestRootCell = GetHighestCompleteRootCell(oldTree, newTree, nodeToCheck.Pointer[direction]);
                    SwapIfHigher(ref highestCompleteRootCell, thisHighestRootCell);
                }
            }

            return highestCompleteRootCell;
        }

        private int GetHighestDoubledCell(CellTree oldTree, CellTree newTree, TreeNode nodeToCheck)
        {
            int highestDoubledCell = 0;
            int thisDoubledCell;

            if (!nodeToCheck.Equals(new TreeNode()))
            {
                //Find the node in new tree at the same position.
                TreeNode mirrorOnNewTree = newTree.NodesInChain.Find(t => t.Position.Equals(nodeToCheck.Position));

                if (!mirrorOnNewTree.Equals(new TreeNode()))
                {
                    if (mirrorOnNewTree.Value == nodeToCheck.Value * 2)
                    {
                        thisDoubledCell = mirrorOnNewTree.Value;
                        highestDoubledCell += thisDoubledCell;
                        //SwapIfHigher(ref highestDoubledCell, thisDoubledCell);
                    }
                }

                foreach (DragDirection direction in DragDirections.AllDragDirections)
                {
                    thisDoubledCell = GetHighestDoubledCell(oldTree, newTree, nodeToCheck.Pointer[direction]);
                    highestDoubledCell += thisDoubledCell;
                    //SwapIfHigher(ref highestDoubledCell, thisDoubledCell);
                }
            }

            return highestDoubledCell;
        }

        private void SwapIfHigher(ref int holder, int value)
        {
            if (value > holder)
            {
                holder = value;
            }
        }

        public CellTree GetCellTree(PlayingGrid Grid)
        {
            CellTree tree = new CellTree(Grid);
            return tree;
        }

        public bool IsMoveDangerous(DragDirection dragDirection)
        {
            bool moveDangerous = false;
            List<DragDirection> allowableDragDirections = new List<DragDirection>();
            int emptyCells;
            int safeChannels, dangerousChannels;

            //Make the move
            PlayingGrid trialGrid = TryMove(dragDirection, this.Grid);

            //Analyse the grid afterwards.
            safeChannels = 0;
            dangerousChannels = 0;
            //check the rows:
            for (int row = 0; row < 4; row++)
            {
                emptyCells = 0;
                for (int column = 0; column < 4; column++)
                {
                    if (trialGrid[row, column] == 0)
                    {
                        emptyCells++;
                    }
                }
                if (emptyCells == 0 && row == 0)
                {
                    allowableDragDirections.Add(DragDirection.Up);
                }
                if (emptyCells == 0 && row == 3)
                {
                    allowableDragDirections.Add(DragDirection.Down);
                }
                if (emptyCells == 2 || emptyCells == 3)
                {
                    safeChannels++;
                }
                if (emptyCells == 1)
                {
                    dangerousChannels++;
                }
            }
            if (safeChannels == 0 && dangerousChannels == 1)
            {
                moveDangerous = true;
            }

            safeChannels = 0;
            dangerousChannels = 0;
            //check the columns:
            for (int column = 0; column < 4; column++)
            {
                emptyCells = 0;
                for (int row = 0; row < 4; row++)
                {
                    if (trialGrid[row, column] == 0)
                    {
                        emptyCells++;
                    }
                }
                if (emptyCells == 0 && column == 0)
                {
                    allowableDragDirections.Add(DragDirection.Left);
                }
                if (emptyCells == 0 && column == 3)
                {
                    allowableDragDirections.Add(DragDirection.Right);
                }
                if (emptyCells == 2 || emptyCells == 3)
                {
                    safeChannels++;
                }
                if (emptyCells == 1)
                {
                    dangerousChannels++;
                }

            }
            if (safeChannels == 0 && dangerousChannels == 1)
            {
                moveDangerous = true;
            }

            //If we can move in any one of the allowable directions then the move is OK;
            foreach (DragDirection direction in allowableDragDirections)
            {
                if (trialGrid.CanMove[direction])
                {
                    return false;
                }
            }

            //If we can merge in any direction then the move is ok:
            foreach (DragDirection direction in DragDirections.AllDragDirections)
            {
                for (int row = 0; row < 4; row++)
                {
                    for (int column = 0; column < 4; column++)
                    {
                        if (trialGrid.Cells[row, column].CanMergeInDirection[direction])
                        {
                            return false;
                        }
                    }
                }
            }

            //otherwise, the move is defined by whether the grid is now dangerous:
            return moveDangerous;
        }

        internal event EventHandler<CellTree> TreeFound;
    }
}