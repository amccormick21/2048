using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048Game.GamePlay
{
    public struct GridRecord
    {
        public Board.PlayingGrid Grid;
        public int score;

        public GridRecord(Board.PlayingGrid Grid, int score)
        {
            this.Grid = Grid;
            this.score = score;
        }
    }
}
