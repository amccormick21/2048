using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace V22048Game.MoveInformation
{
    struct MoveResults
    {
        public MoveEventArgs NextMoveEventArgs;
        public int ScoreDelta;
        public int HighestBlock;
        public bool CanStillMove;
    }
}
