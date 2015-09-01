using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2048Game.Board;

namespace _2048Game
{
    public class ViewModel
    {
        public delegate void CellMovedEventHandler(Cell OldCell, Cell NewCell);
        public event CellMovedEventHandler CellMoved;

        public void OnCellMoved(Cell OldCell, Cell NewCell)
        {
            if (CellMoved != null)
                CellMoved(OldCell, NewCell);
        }
    }
}
