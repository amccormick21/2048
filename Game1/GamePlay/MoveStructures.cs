using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048Game.Board
{
    public struct PositionOnBoard : IEquatable<PositionOnBoard>, IComparable<PositionOnBoard>
    {
        public int Row;
        public int Column;

        public PositionOnBoard(int row, int column)
        {
            this.Row = row;
            this.Column = column;
        }

        public static PositionOnBoard nullPosition = new PositionOnBoard(-1, -1);

        public bool Equals(PositionOnBoard other)
        {
            return (Row == other.Row && Column == other.Column);
        }

        /// <summary>
        /// Returns a larger number if the cell is closer to (3,0), by row first, then column.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(PositionOnBoard other)
        {
            if (other.Equals(nullPosition))
            { return 17; }
            else
            {
                if (this.Equals(nullPosition))
                { return -17; }
            }

            if (this.Row != other.Row)
            {
                return 4 * (this.Row - other.Row);
            }
            else
            {
                return (other.Column - this.Column);
            }
        }
    }

    public struct Move
    {
        public int SourceRow;
        public int SourceColumn;
        public int DestinationRow;
        public int DestinationColumn;

        public Move(int sourceRow, int sourceCol, int destRow, int destCol)
        {
            this.SourceRow = sourceRow;
            this.SourceColumn = sourceCol;
            this.DestinationRow = destRow;
            this.DestinationColumn = destCol;
        }

        public Move(PositionOnBoard startPosition, PositionOnBoard endPosition)
        {
            this.SourceRow = startPosition.Row;
            this.SourceColumn = startPosition.Column;
            this.DestinationRow = endPosition.Row;
            this.DestinationColumn = endPosition.Column;
        }

        public PositionOnBoard StartPosition
        {
            get { return new PositionOnBoard(SourceRow, SourceColumn); }
            set { this.SourceRow = value.Row; this.SourceColumn = value.Column; }
        }
        public PositionOnBoard EndPosition
        {
            get { return new PositionOnBoard(DestinationRow, DestinationColumn); }
            set { this.DestinationRow = value.Row; this.DestinationColumn = value.Column; }
        }
    }

    public class MoveMadeEventArgs : EventArgs
    {
        public List<Move> Moves;
        public List<PositionOnBoard> ValuesChanged;
        public Cell NewCell;

        public MoveMadeEventArgs(List<Move> Moves, List<PositionOnBoard> ValuesChanged, Cell NewCell)
        {
            this.Moves = Moves;
            this.ValuesChanged = ValuesChanged;
            this.NewCell = NewCell;
        }

        public MoveMadeEventArgs()
        { }
    }
}
