using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V22048Game.Elements
{
    class GridRecord
    {
        internal Board Board { get; private set;}
        internal int Score { get; private set;}
        internal int HighBlock { get; private set; }

        public GridRecord(Board board, int score, int highBlock)
        {
            Board = board;
            Score = score;
            HighBlock = highBlock;
        }
    }
}
