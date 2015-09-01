using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V22048Game.Elements
{
    public struct Position : IEquatable<Position>
    {
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

        int row, column;

        public Position(int Row, int Column)
        {
            row = Row;
            column = Column;
        }

        public bool Equals(Position other)
        {
            return (this.Row == other.Row && this.Column == other.Column);
        }

        public override string ToString()
        {
            return "(" + Convert.ToString(Row) + "," + Convert.ToString(Column) + ")";
        }
    }
}
