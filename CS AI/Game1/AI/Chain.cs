using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2048Game.Board;

namespace _2048Game.AI
{
    public struct ChainComponent
    {
        Cell cell;
        DragDirection directionOfNext;
        bool isBase;

        public ChainComponent(Cell Cell, DragDirection DirectionOfNext)
        {
            this.cell = Cell;
            this.directionOfNext = DirectionOfNext;
            this.isBase = false;
        }
        public ChainComponent(Cell Cell)
        {
            this.cell = Cell;
            this.directionOfNext = DragDirection.Left;
            this.isBase = true;
        }

        public Cell Cell
        {
            get { return cell; }
            set { cell = value; }
        }

        /// <summary>
        /// <para>Gets the direction of the next cell in the chain, i.e. 'down' if the next (higher) cell is below.</para>
        /// <para>Will return left if the cell is the base cell of the chain. This is to be ignored.</para>
        /// </summary>
        public DragDirection DirectionOfNext
        {
            get
            {
                return directionOfNext;
            }
        }
        public bool IsBase
        {
            get { return isBase; }
            set { isBase = value; }
        }
    }

    public class Chain : Stack<ChainComponent>
    {
        PlayingGrid hostGrid;

        public Chain(Cell BaseOfChain, PlayingGrid HostGrid)
        {
            hostGrid = HostGrid;
            AddCell(BaseOfChain);
        }

        public Chain(Chain chainCopy)
        {
            Stack<ChainComponent> intermediateChain = new Stack<ChainComponent>();

            for (int i = 0; i < chainCopy.Count; i++)
            {
                intermediateChain.Push(chainCopy.Pop());
            }
            for (int i = 0; i < intermediateChain.Count; i++)
            {
                this.Push(intermediateChain.Pop());
            }

            this.hostGrid = new PlayingGrid(chainCopy.hostGrid);
        }

        /// <summary>
        /// Pushes the base cell onto the stack
        /// </summary>
        /// <param name="TargetCell">The base cell in the chain</param>
        private void AddCell(Cell TargetCell)
        {
            //Adds the base cell
            this.Push(new ChainComponent(TargetCell));

            //Then finds the next cell
            DragDirection directionOfNext;
            bool equalFound;
            bool halfFound;
            Cell chosenCell;

            chosenCell = FindNextCellInChain(TargetCell, out directionOfNext, out equalFound, out halfFound);

            //And works out what to do with it.
            if (equalFound)
            {
                this.Push(new ChainComponent(chosenCell, DragDirections.Opposite(directionOfNext)));
            }
            else
            {
                if (halfFound)
                {
                    AddCell(chosenCell, directionOfNext);
                }
            }
        }

        /// <summary>
        /// Pushes a general cell onto the stack
        /// </summary>
        /// <param name="TargetCell">The cell to be pushed onto the stack</param>
        /// <param name="DirectionOfLast">The direction of the cell before the target cell</param>
        private void AddCell(Cell TargetCell, DragDirection DirectionOfLast)
        {
            //Adds this cell
            this.Push(new ChainComponent(TargetCell, DragDirections.Opposite(DirectionOfLast)));

            //Then finds the next cell
            DragDirection directionOfNext;
            bool equalFound;
            bool halfFound;
            Cell chosenCell;

            chosenCell = FindNextCellInChain(TargetCell, out directionOfNext, out equalFound, out halfFound);


            //And works out what to do with it.
            if (equalFound)
            {
                this.Push(new ChainComponent(chosenCell, DragDirections.Opposite(directionOfNext)));
            }
            else
            {
                if (halfFound)
                {
                    AddCell(chosenCell, directionOfNext);
                }
            }
        }

        private Cell FindNextCellInChain(Cell TargetCell, out DragDirection directionOfNext, out bool equalFound, out bool halfFound)
        {
            equalFound = false;
            halfFound = false;
            List<Cell> equalCells = new List<Cell>();
            List<Cell> halfCells = new List<Cell>();
            List<DragDirection> equalDirections = new List<DragDirection>();
            List<DragDirection> halfDirections = new List<DragDirection>();

            directionOfNext = DragDirection.Left;

            bool isEqualTo, allHigherThan;
            int highestLessThan;

            Cell checkCell = new Cell();
            Cell chosenCell = new Cell();

            foreach (var Direction in DragDirections.AllDragDirections)
            {
                if (hostGrid.GetCellInDirection(TargetCell, Direction, out checkCell))
                {
                    //if (!checkCell.InChain)
                    //{
                        if (checkCell.Value == TargetCell.Value)
                        {
                            equalFound = true;
                            equalCells.Add(checkCell);
                            equalDirections.Add(Direction);
                            chosenCell = checkCell;
                        }
                        else
                        {
                            if (checkCell.Value == TargetCell.Value / 2)
                            {
                                bool surrounded = checkCell.IsSurrounded(out isEqualTo, out allHigherThan, out highestLessThan, this.hostGrid);
                                if (!allHigherThan)
                                {
                                    halfFound = true;
                                    halfCells.Add(checkCell);
                                    halfDirections.Add(Direction);
                                    chosenCell = checkCell;
                                }
                            }
                        }
                    //}
                }
            }

            int longestChainHolder = 0;
            Cell bestCell = new Cell();
            Chain tempChain;

            if (equalFound)
            {
                directionOfNext = equalDirections[0];
                return chosenCell;
            }
            else
            {
                if (halfFound && halfCells.Count != 1)
                {
                    int halfCellIndex = 0;
                    foreach (Cell cell in halfCells)
                    {
                        tempChain = new Chain(cell, hostGrid);
                        if (tempChain.Count > longestChainHolder)
                        {
                            longestChainHolder = tempChain.Count;
                            bestCell = cell;
                            directionOfNext = halfDirections[halfCellIndex];
                        }
                        halfCellIndex++;
                    }
                    return bestCell;
                }
                else
                {
                    return chosenCell;
                }
            }
        }

        public ChainComponent this[int index]
        {
            get { return this.ElementAt(index); }
        }

        public List<DragDirection> GetPossibleDirections(ICollection<DragDirection> dragDirections)
        {
            List<DragDirection> safeDirections = new List<DragDirection>();
            bool safeToMove;

            foreach (DragDirection direction in dragDirections)
            {
                safeToMove = true;
                foreach (ChainComponent component in this)
                {
                    if (component.Cell.MaxScaleInDirection[direction] != 0)
                    {
                        if (!component.IsBase)
                        {
                            if (component.DirectionOfNext != direction)
                            {
                                safeToMove = false;
                            }
                        }
                        else
                        {
                            safeToMove = false;
                        }
                    }
                }
                if (safeToMove && hostGrid.CanMove[direction])
                {
                    safeDirections.Add(direction);
                }
            }

            return safeDirections;
        }

        public ChainComponent GetLastCellInChain()
        {
            return this.Peek();
        }
    }
}
