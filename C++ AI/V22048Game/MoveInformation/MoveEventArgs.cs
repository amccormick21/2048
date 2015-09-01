using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V22048Game.Elements;

namespace V22048Game.MoveInformation
{
    public class MoveEventArgs : EventArgs
    {
        public List<Move> Moves { get; set; }
        public List<Cell> DoubledCells { get; set; }
        public List<Cell> NewCells { get; set; }

        public MoveEventArgs()
        {

        }

        public MoveEventArgs(List<Move> moves, List<Cell> doubledCells, List<Cell> newCells)
            : base()
        {
            Moves = moves;
            DoubledCells = doubledCells;
            NewCells = newCells;
        }
    }
}
