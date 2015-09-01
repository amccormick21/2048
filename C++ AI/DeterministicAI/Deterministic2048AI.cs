using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using V22048Game.Elements;
using V22048Game.MoveInformation;
using Library2048AI;
using System.Reflection;

namespace DeterministicAI
{
    public class Deterministic2048AI : IAi
    {
        #region Marshalling Structures
        const int arrayLength = 4;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct Cell
        {
            int value;
            int row;
            int column;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = arrayLength)]
            int[] nextPosition;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = arrayLength)]
            int[] nextMoveAction;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = arrayLength)]
            int[] nextMoveMergeValue;

            public Cell(V22048Game.Elements.Cell cell)
            {
                value = (int)cell.Value.Real;
                row = cell.Row;
                column = cell.Column;
                nextPosition = new int[4];
                nextMoveAction = new int[4];
                nextMoveMergeValue = new int[4];
                for (int i = 0; i < 4; i++)
                {
                    nextPosition[i] = (int)cell.NextPosition[(Direction)i];
                    nextMoveAction[i] = (int)cell.NextMoveAction[(Direction)i];
                    nextMoveMergeValue[i] = (int)cell.NextMoveMergeValue[(Direction)i].Real;
                }
            }
        }
        
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct Board
        {
            int gridSize;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = arrayLength * arrayLength)]
            Cell[] cells;

            public Board(V22048Game.Elements.Board board)
            {
                gridSize = board.GridSize;
                cells = new Cell[board.Cells.Length];
                foreach (var cell in board.Cells)
                {
                    cells[cell.Column + cell.Row * gridSize] = new Cell(cell);
                }
            }
        }
        #endregion

        Direction preparedMove;

        public Deterministic2048AI()
        { }

        public void PrepareMove(V22048Game.Elements.Board board)
        {
            Board newBoard = new Board(board);
            int iSize = Marshal.SizeOf(typeof(Board));

            IntPtr pNewBoard = Marshal.AllocHGlobal(iSize);
            Marshal.StructureToPtr(newBoard, pNewBoard, false);

            //TODO: exception handling here
            preparedMove = (Direction)GetMove(pNewBoard);

            Marshal.FreeHGlobal(pNewBoard);

            if (MoveReady != null)
                MoveReady(this, new EventArgs());
        }

        [DllImport(@"C:\Users\Alex\SkyDrive\Documents\Projects\2048\v2\V2.6\DeterministicAI\bin\Debug\Library2048AI.dll", CallingConvention = CallingConvention.StdCall, EntryPoint = "?getMove@AIMethods@Library2048AI@@QAEHPAUboard@@@Z")]
        private static extern int GetMove(IntPtr pBoard);

        public Direction GetPreparedMove()
        {
            return preparedMove;
        }

        public event EventHandler MoveReady;
    }
}
