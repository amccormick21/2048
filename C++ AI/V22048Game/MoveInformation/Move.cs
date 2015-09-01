using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V22048Game.Elements;

namespace V22048Game.MoveInformation
{
    public class Move
    {
        public Position StartPosition { get; internal set; }
        public Position EndPosition { get; internal set; }

        public Move()
        {
        }

        public Move(Position startPosition, Position endPosition)
        {
            StartPosition = startPosition;
            EndPosition = endPosition;
        }

        public Move(int startRow, int startColumn, int endRow, int endColumn)
        {
            StartPosition = new Position(startRow, startColumn);
            EndPosition = new Position(endRow, endColumn);
        }
    }
}
