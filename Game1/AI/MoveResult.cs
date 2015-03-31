using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048Game.AI
{
    struct MoveResult
    {
        public DragDirection Direction;
        public string Reason;

        public MoveResult(DragDirection Direction, string Reason)
        {
            this.Direction = Direction;
            this.Reason = Reason;
        }

    }
}
