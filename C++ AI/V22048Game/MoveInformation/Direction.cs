using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V22048Game.MoveInformation
{
    public enum Direction
    {
        Left,
        Up,
        Right,
        Down
    }

    public static class Directions
    {
        public static Direction Opposite(this Direction direction)
        {
            int opposite = ((int)direction + 2) >> 1;
            return (Direction)opposite;
        }

        public static Level Level(this Direction direction)
        {
            return (Level)(((int)direction) & 1);
        }

        public static Direction[] All = new Direction[] { Direction.Left, Direction.Up, Direction.Right, Direction.Down };
    }
}
