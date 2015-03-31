using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048Game.GamePlay
{
    public class GameOverEventArgs : EventArgs
    {
        public int Score {get; set;}
        public int HighestTile { get; set; }

        public GameOverEventArgs(int Score, int HighestTile)
        {
            this.Score = Score;
            this.HighestTile = HighestTile;
        }
    }
}
