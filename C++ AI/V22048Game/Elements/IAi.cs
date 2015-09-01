using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V22048Game.MoveInformation;

namespace V22048Game.Elements
{
    public interface IAi
    {
        Direction GetPreparedMove();
        void PrepareMove(Board board);
        event EventHandler MoveReady;
    }
}
