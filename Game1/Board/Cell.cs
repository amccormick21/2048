using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2048Game.AI;

namespace _2048Game.Board
{
    public struct Cell : IEquatable<Cell>, IComparable<Cell>
    {
        int row;
        int column;
        int value;
        bool inTree;
        Dictionary<DragDirection, int> maxScaleInDirection;
        Dictionary<DragDirection, bool> canMergeInDirection;

        public Cell(int row, int column, int value)
        {
            this.row = row;
            this.column = column;
            this.value = value;
            this.inTree = false;
            this.maxScaleInDirection = new Dictionary<DragDirection, int>(4);
            foreach (var Direction in DragDirections.AllDragDirections)
            {
                this.maxScaleInDirection[Direction] = 0;
            }
            this.canMergeInDirection = new Dictionary<DragDirection, bool>(4);
            foreach (var Direction in DragDirections.AllDragDirections)
            {
                this.canMergeInDirection[Direction] = false;
            }
        }

        public int Value
        {
            get { return value; }
            set { this.value = value; }
        }

        public bool InTree
        {
            get { return inTree; }
            set { inTree = value; }
        }

        public Dictionary<DragDirection,int> MaxScaleInDirection
        {
            get { return maxScaleInDirection; }
            set { maxScaleInDirection = value; }
        }

        public Dictionary<DragDirection, bool> CanMergeInDirection
        {
            get { return canMergeInDirection; }
            set { canMergeInDirection = value; }
        }

        public int Row
        {
            get { return row; }
            set { row = value; }
        }
        public int Column
        {
            get { return column; }
            set { column = value; }
        }

        public PositionOnBoard Position
        {
            get { return new PositionOnBoard(Row, Column); }
            set { Row = value.Row; Column = value.Column; }
        }

        public bool IsSurrounded(out bool isEqualTo, out bool allHigherThan, out int highestLessThan, PlayingGrid hostGrid)
        {
            isEqualTo = false;
            allHigherThan = true;
            bool isSurrounded = true;
            highestLessThan = 0;
            Cell checkCell = new Cell();

            foreach (DragDirection direction in DragDirections.AllDragDirections)
            {
                bool cellFound = hostGrid.GetCellInDirection(this, direction, out checkCell);
                if (cellFound)
                {
                    if (checkCell.Value == this.Value)
                    {
                        isEqualTo = true;
                        allHigherThan = false;
                    }
                    else
                    {
                        if (checkCell.Value < this.Value)
                        {
                            allHigherThan = false;
                            if (checkCell.Value == 0)
                            {
                                isSurrounded = false;
                            }
                            if (checkCell.Value > highestLessThan)
                            {
                                highestLessThan = checkCell.Value;
                            }
                        }
                    }
                }
            }

            return isSurrounded;
        }

        internal Cell GetHighestCellLowerThan(PlayingGrid hostGrid)
        {
            int highestValueLessThan = -1;
            Cell cellHolder = new Cell();
            Cell checkCell;
            bool canGetCellInDirection;

            foreach (DragDirection direction in DragDirections.AllDragDirections)
            {
                canGetCellInDirection = hostGrid.GetCellInDirection(this, direction, out checkCell);
                if (canGetCellInDirection)
                {
                    if (checkCell.Value < this.value)
                    {
                        if (checkCell.Value > highestValueLessThan)
                        {
                            highestValueLessThan = checkCell.Value;
                            cellHolder = checkCell;
                        }
                    }
                }
            }

            return cellHolder;
        }

        internal Cell GetLowestCellHigherThan(ICollection<DragDirection> searchDirections, PlayingGrid hostGrid)
        {
            int highestValueLessThan = 0;
            Cell cellHolder = new Cell();
            Cell checkCell;
            bool canGetCellInDirection;
            int directionsChecked = 0;

            foreach (DragDirection direction in searchDirections)
            {
                canGetCellInDirection = hostGrid.GetCellInDirection(this, direction, out checkCell);
                if (canGetCellInDirection)
                {
                    if (directionsChecked++ == 0)
                    {
                        highestValueLessThan = checkCell.Value;
                    }
                    if (checkCell.Value > this.value)
                    {
                        if (checkCell.Value < highestValueLessThan)
                        {
                            highestValueLessThan = checkCell.Value;
                            cellHolder = checkCell;
                        }
                    }
                }
            }

            return cellHolder;
        }

        internal Cell GetHighestCellHigherThan(ICollection<DragDirection> searchDirections, PlayingGrid hostGrid)
        {
            int highestValueLessThan = 0;
            Cell cellHolder = new Cell();
            Cell checkCell;
            bool canGetCellInDirection;
            int directionsChecked = 0;

            foreach (DragDirection direction in searchDirections)
            {
                canGetCellInDirection = hostGrid.GetCellInDirection(this, direction, out checkCell);
                if (canGetCellInDirection)
                {
                    if (directionsChecked++ == 0)
                    {
                        highestValueLessThan = checkCell.Value;
                    }
                    if (checkCell.Value > this.value)
                    {
                        if (checkCell.Value > highestValueLessThan)
                        {
                            highestValueLessThan = checkCell.Value;
                            cellHolder = checkCell;
                        }
                    }
                }
            }

            return cellHolder;
        }


        /// <summary>
        /// Returns a larger value if the cell has a larger value than other.
        /// If values are equal, returns a larger number if the cell is closer to (3,0)
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(Cell other)
        {
            if (this.Value != other.Value)
            {
                return 16 * (this.Value - other.Value);
            }
            else
            {
                if (this.Row != other.Row)
                {
                    return 4 * (this.Row - other.Row);
                }
                else
                {
                    return (other.column - this.Column);
                }
            }
        }

        public bool Equals(Cell other)
        {
            if (this.Value == other.Value)
            {
                if (this.Row == other.Row)
                {
                    if (this.Column == other.Column)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        internal Cell GetCellToBuildChainOn(List<DragDirection> searchDirections, PlayingGrid hostGrid)
        {
            bool isEqualTo, allHigherThan;
            int highestValueLessThan;
            Cell cellHolder = new Cell();
            Cell checkCell;
            bool canGetCellInDirection;
            int leastValueSurroundingHolder = GetHighestCellHigherThan(searchDirections, hostGrid).Value;

            foreach (DragDirection direction in searchDirections)
            {
                canGetCellInDirection = hostGrid.GetCellInDirection(this, direction, out checkCell);
                if (canGetCellInDirection)
                {
                    checkCell.IsSurrounded(out isEqualTo, out allHigherThan, out highestValueLessThan, hostGrid);
                    if (!allHigherThan)
                    {
                        if (leastValueSurroundingHolder > checkCell.Value)
                        {
                            leastValueSurroundingHolder = checkCell.Value;
                            cellHolder = checkCell;
                        }
                    }
                }
            }

            return cellHolder;
        }

        internal Cell Clone()
        {
            return (Cell)this.MemberwiseClone();
        }
    }
}
