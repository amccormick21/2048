using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048Game.Board
{
    public class PlayingGrid
    {
        Cell[,] cells;
        Dictionary<DragDirection, bool> canMove;

        public PlayingGrid()
        {
            InitialiseCells();
            canMove = InitialiseCanMove();
        }
        public PlayingGrid(PlayingGrid oldGrid)
        {
            InitialiseCells();
            canMove = InitialiseCanMove();
            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    Cells[row, column] = oldGrid.Cells[row, column];
                }
            }

            canMove = this.GetCanMoveInDirection();
        }

        private Dictionary<DragDirection, bool> InitialiseCanMove()
        {
            Dictionary<DragDirection, bool> Temp = new Dictionary<DragDirection, bool>();
            Temp[DragDirection.Left] = false;
            Temp[DragDirection.Up] = false;
            Temp[DragDirection.Right] = false;
            Temp[DragDirection.Down] = false;

            return Temp;
        }

        private void InitialiseCells()
        {
            cells = new Cell[4, 4];
            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    cells[row, column] = new Cell(row, column, 0);
                }
            }
        }

        public bool SetCanSizeInDirection(DragDirection dragDirection)
        {
            return CanMoveInDirection(dragDirection);
        }
        public void SetCanSizeInDirection()
        {
            this.CanMove = GetCanMoveInDirection();
        }

        public int CountEmptyCells()
        {
            int emptyCells = 0;
            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    if (Cells[row, column].Value == 0)
                        emptyCells++;
                }
            }

            return emptyCells;
        }

        /// <summary>
        /// Returns a dictionary of values specifying whether the grid can be moved in a particlar direction
        /// </summary>
        private Dictionary<DragDirection, bool> GetCanMoveInDirection()
        {
            Dictionary<DragDirection, bool> canMove = new Dictionary<DragDirection, bool>();

            foreach (var Direction in DragDirections.AllDragDirections)
            {
                canMove[Direction] = CanMoveInDirection(Direction);
            }

            return canMove;
        }

        /// <summary>
        /// Returns whether the grid can move in a particular direction
        /// </summary>
        /// <param name="direction">The direction to check if the grid can move</param>
        private bool CanMoveInDirection(DragDirection direction)
        {
            // Reset the can merge parameters on cells
            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    this.Cells[row, column].CanMergeInDirection[direction] = false;
                }
            }

            int checkRow, checkColumn;
            int emptyCellsInConstantChannel;
            int previousCellValue;
            bool canMove = false;
            bool canMerge = false;

            for (int variableIndex = 0; variableIndex < 4; variableIndex++)
            {
                if (direction == DragDirection.Left)
                {
                    //LEFT
                    checkColumn = 0;
                    checkRow = variableIndex;
                    emptyCellsInConstantChannel = 0;
                    previousCellValue = 0;
                    while (checkColumn < 4)
                    {
                        if (this[checkRow, checkColumn] == 0)
                        {
                            emptyCellsInConstantChannel++;
                        }
                        else
                        {
                            SetCellMoveLimits(direction, checkRow, checkColumn, ref emptyCellsInConstantChannel, ref previousCellValue, ref canMove, ref canMerge);
                        }
                        checkColumn++;
                    }
                }

                if (direction == DragDirection.Up)
                {
                    //UP
                    checkRow = 0;
                    checkColumn = variableIndex;
                    emptyCellsInConstantChannel = 0;
                    previousCellValue = 0;
                    while (checkRow < 4)
                    {
                        if (this[checkRow, checkColumn] == 0)
                        {
                            emptyCellsInConstantChannel++;
                        }
                        else
                        {
                            SetCellMoveLimits(direction, checkRow, checkColumn, ref emptyCellsInConstantChannel, ref previousCellValue, ref canMove, ref canMerge);
                        }
                        checkRow++;
                    }
                }

                if (direction == DragDirection.Right)
                {
                    //RIGHT
                    checkColumn = 3;
                    checkRow = variableIndex;
                    emptyCellsInConstantChannel = 0;
                    previousCellValue = 0;
                    while (checkColumn >= 0)
                    {
                        if (this[checkRow, checkColumn] == 0)
                        {
                            emptyCellsInConstantChannel++;
                        }
                        else
                        {
                            SetCellMoveLimits(direction, checkRow, checkColumn, ref emptyCellsInConstantChannel, ref previousCellValue, ref canMove, ref canMerge);
                        }
                        checkColumn--;
                    }
                }

                if (direction == DragDirection.Down)
                {
                    //DOWN
                    checkRow = 3;
                    checkColumn = variableIndex;
                    emptyCellsInConstantChannel = 0;
                    previousCellValue = 0;
                    while (checkRow >= 0)
                    {
                        if (this[checkRow, checkColumn] == 0)
                        {
                            emptyCellsInConstantChannel++;
                        }
                        else
                        {
                            SetCellMoveLimits(direction, checkRow, checkColumn, ref emptyCellsInConstantChannel, ref previousCellValue, ref canMove, ref canMerge);
                        }
                        checkRow--;
                    }
                }
            }

            return (canMove || canMerge);
        }

        /// <summary>
        /// Sets the distance that a cell can move in a particular direction
        /// </summary>
        /// <param name="direction">The direction to check for move distance</param>
        /// <param name="checkRow">The row containing the cell</param>
        /// <param name="checkColumn">The column containing the cell</param>
        /// <param name="emptyCellsInConstantChannel">The cumulative number of empty cells found in the row or column so far</param>
        /// <param name="previousCellValue">The value of the previous cell found</param>
        /// <param name="canMove">Represents if the cell can move</param>
        /// <param name="canMerge">Represents whether the cell can merge with another cell</param>
        private void SetCellMoveLimits(DragDirection direction, int checkRow, int checkColumn, ref int emptyCellsInConstantChannel, ref int previousCellValue, ref bool canMove, ref bool canMerge)
        {
            bool mergeOccurred = false;
            if (emptyCellsInConstantChannel != 0)
                canMove = true;
            if (this[checkRow, checkColumn] == previousCellValue)
            {
                canMerge = true;
                if (previousCellValue != 0)
                {
                    Cells[checkRow, checkColumn].CanMergeInDirection[direction] = true;
                }
                emptyCellsInConstantChannel++;
                mergeOccurred = true;
            }
            Cells[checkRow, checkColumn].MaxScaleInDirection[direction] = emptyCellsInConstantChannel;
            previousCellValue = (mergeOccurred ? 0 : this[checkRow, checkColumn]);
        }

        public List<Move> MoveAllCells(DragDirection dragDirection, out List<PositionOnBoard> valuesChanged, ref int ScoreDelta)
        {
            int constantLocation;
            List<Move> MovesToMake = new List<Move>();
            List<PositionOnBoard> PositionsOfValueChanged = new List<PositionOnBoard>();

            switch (dragDirection)
            {
                case DragDirection.Left:
                case DragDirection.Up:
                    constantLocation = 0; break;
                
                case DragDirection.Right:
                case DragDirection.Down:
                    constantLocation = 3; break;

                default: constantLocation = 0; break;
            }

            for (int variableIndex = 0; variableIndex != 4; variableIndex++)
            {
                MoveCells(dragDirection, constantLocation, variableIndex, ref MovesToMake, ref PositionsOfValueChanged, ref ScoreDelta);
            }

            valuesChanged = new List<PositionOnBoard>(PositionsOfValueChanged);

            return MovesToMake;
        }

        internal void WriteCellsForDebug()
        {
            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    Console.Out.Write(Cells[row, column].Value);
                    Console.Out.Write(" ");
                }
                Console.Out.WriteLine();
            }
            Console.Out.WriteLine();
        }

        private void MoveCells(DragDirection dragDirection, int constantLocation, int variableIndex, ref List<Move> MovesToMake, ref List<PositionOnBoard> PositionsToUpdate, ref int ScoreDelta)
        {
            int checkRow, checkColumn;
            switch (dragDirection)
            {
                case DragDirection.Left:
                    checkColumn = constantLocation;
                    checkRow = variableIndex;
                    while (checkColumn < 4)
                    {
                        if (this[checkRow, checkColumn] != 0 && Cells[checkRow, checkColumn].MaxScaleInDirection[dragDirection] != 0)
                        {
                            MovesToMake.Add(new Move(checkRow, checkColumn, checkRow, checkColumn - Cells[checkRow, checkColumn].MaxScaleInDirection[dragDirection]));
                            SwapCellsOnColumn(Cells[checkRow, checkColumn], -Cells[checkRow, checkColumn].MaxScaleInDirection[dragDirection], ref PositionsToUpdate, ref ScoreDelta);
                        }
                        checkColumn++;
                    }
                    break;
                case DragDirection.Up:
                    checkRow = constantLocation;
                    checkColumn = variableIndex;
                    while (checkRow < 4)
                    {
                        if (this[checkRow, checkColumn] != 0 && Cells[checkRow, checkColumn].MaxScaleInDirection[dragDirection] != 0)
                        {
                            MovesToMake.Add(new Move(checkRow, checkColumn, checkRow - Cells[checkRow, checkColumn].MaxScaleInDirection[dragDirection], checkColumn));
                            SwapCellsOnRow(Cells[checkRow, checkColumn], -Cells[checkRow, checkColumn].MaxScaleInDirection[dragDirection], ref PositionsToUpdate, ref ScoreDelta);
                        }
                        checkRow++;
                    }
                    break;
                case DragDirection.Right:
                    checkColumn = constantLocation;
                    checkRow = variableIndex;
                    while (checkColumn >= 0)
                    {
                        if (this[checkRow, checkColumn] != 0 && Cells[checkRow, checkColumn].MaxScaleInDirection[dragDirection] != 0)
                        {
                            MovesToMake.Add(new Move(checkRow, checkColumn, checkRow, checkColumn + Cells[checkRow, checkColumn].MaxScaleInDirection[dragDirection]));
                            SwapCellsOnColumn(Cells[checkRow, checkColumn], Cells[checkRow, checkColumn].MaxScaleInDirection[dragDirection], ref PositionsToUpdate, ref ScoreDelta);
                        }
                        checkColumn--;
                    }
                    break;
                case DragDirection.Down:
                    checkRow = constantLocation;
                    checkColumn = variableIndex;
                    while (checkRow >= 0)
                    {
                        if (this[checkRow, checkColumn] != 0 && Cells[checkRow, checkColumn].MaxScaleInDirection[dragDirection] != 0)
                        {
                            MovesToMake.Add(new Move(checkRow, checkColumn, checkRow + Cells[checkRow, checkColumn].MaxScaleInDirection[dragDirection], checkColumn));
                            SwapCellsOnRow(Cells[checkRow, checkColumn], Cells[checkRow, checkColumn].MaxScaleInDirection[dragDirection], ref PositionsToUpdate, ref ScoreDelta);
                        }
                        checkRow--;
                    }
                    break;
            }
        }

        /// <summary>
        /// <para>Moves and copies the parameters of a cell a specified number of positions along a row.</para>
        /// <para>Updates the row fields on both cells.</para>
        /// </summary>
        /// <param name="CellToSwap">The cell that is being moved</param>
        /// <param name="RowChange">The change from the original row that is being implemented</param>
        private void SwapCellsOnRow(Cell CellToSwap, int RowChange, ref List<PositionOnBoard> PositionsToUpdate, ref int ScoreDelta)
        {
            int currentRow = CellToSwap.Row;
            int column = CellToSwap.Column;
            int newRow = currentRow + RowChange;
            int temp;
            bool doubleValue;

            //Find out if value will be doubled in swap:
            doubleValue = (Cells[currentRow, column].Value == Cells[newRow, column].Value);

            //Set destination cell to zero.
            Cells[newRow, column].Value = 0;
            
            //Swap cells
            temp = Cells[newRow, column].Value;
            Cells[newRow, column].Value = Cells[currentRow, column].Value;
            Cells[currentRow, column].Value = temp;

            if (doubleValue)
            {
                Cells[newRow, column].Value *= 2;
                ScoreDelta += Cells[newRow, column].Value;
                PositionsToUpdate.Add(Cells[newRow, column].Position);
            }

            Cells[currentRow, column].Row = currentRow;
            Cells[newRow, column].Row = newRow;
        }

        /// <summary>
        /// <para>Moves and copies the parameters of a cell a specified number of positions along a column.</para>
        /// <para>Updates the column fields on both cells.</para>
        /// </summary>
        /// <param name="CellToSwap">The cell that is being moved</param>
        /// <param name="ColumnChange">The change from the original column that is being implemented</param>
        private void SwapCellsOnColumn(Cell CellToSwap, int ColumnChange, ref List<PositionOnBoard> PositionsToUpdate, ref int ScoreDelta)
        {
            int currentColumn = CellToSwap.Column;
            int row = CellToSwap.Row;
            int newColumn = currentColumn + ColumnChange;
            int temp;
            bool doubleValue;

            //Find out if value will be doubled in swap:
            doubleValue = (Cells[row, currentColumn].Value == Cells[row, newColumn].Value);

            //Set destination cell to zero.
            Cells[row, newColumn].Value = 0;

            //Swap cells
            temp = Cells[row, newColumn].Value;
            Cells[row, newColumn].Value = Cells[row, currentColumn].Value;
            Cells[row, currentColumn].Value = temp;

            if (doubleValue)
            {
                Cells[row, newColumn].Value *= 2;
                ScoreDelta += Cells[row, newColumn].Value;
                PositionsToUpdate.Add(Cells[row, newColumn].Position);
            }

            Cells[row, currentColumn].Column = currentColumn;
            Cells[row, newColumn].Column = newColumn;
        }

        public Cell InsertRandomNewCell(int numberOfEmptyCells)
        {
            Cell CellToInsert = new Cell();
            Random Rand = (App.FixedRandom ? new Random(0) : new Random());
            int randomCell = Rand.Next(0, numberOfEmptyCells);
            int blankCellsFound = 0;

            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    if (this[row, column] == 0)
                    {
                        if (randomCell == blankCellsFound++)
                        {
                            this[row, column] = (Rand.Next(0, 10) == 9 ? 4 : 2);
                            CellToInsert = Cells[row, column];
                        }
                    }
                }
            }

            return CellToInsert;
        }

        public bool IsGameOver()
        {
            bool canMoveInDirection = false;
            foreach (var direction in DragDirections.AllDragDirections)
            {
                if (CanMove[direction]) { canMoveInDirection = true; }
            }
            return !canMoveInDirection;
        }

        public Dictionary<DragDirection, bool> CanMove
        {
            get { return canMove; }
            private set { canMove = value; }
        }

        public Cell[,] Cells
        {
            get { return cells; }
            set { cells = value; }
        }


        internal void ResetGrid()
        {
            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    Cells[row, column].Value = 0;
                }
            }
        }

        public static PositionOnBoard GetPositionInDirection(PositionOnBoard TargetPosition, DragDirection Direction)
        {
            PositionOnBoard NewPosition = PositionOnBoard.nullPosition;

            switch (Direction)
            {
                case DragDirection.Left:
                    if (TargetPosition.Column != 0)
                    {
                        NewPosition = new PositionOnBoard(TargetPosition.Row, TargetPosition.Column - 1);
                    }
                    break;
                case DragDirection.Up:
                    if (TargetPosition.Row != 0)
                    {
                        NewPosition = new PositionOnBoard(TargetPosition.Row - 1, TargetPosition.Column);
                    }
                    break;
                case DragDirection.Right:
                    if (TargetPosition.Column != 3)
                    {
                        NewPosition = new PositionOnBoard(TargetPosition.Row, TargetPosition.Column + 1);
                    }
                    break;
                case DragDirection.Down:
                    if (TargetPosition.Row != 3)
                    {
                        NewPosition = new PositionOnBoard(TargetPosition.Row + 1, TargetPosition.Column);
                    }
                    break;
                default:
                    break;
            }

            return NewPosition;

        }

        public bool GetCellInDirection(Cell TargetCell, DragDirection Direction, out Cell returnedCell)
        {
            returnedCell = new Cell();

            switch (Direction)
            {
                case DragDirection.Left:
                    if (TargetCell.Column == 0)
                    {
                        return false;
                    }
                    else
                    {
                        returnedCell = Cells[TargetCell.Row, TargetCell.Column - 1];
                        return true;
                    }
                case DragDirection.Up:
                    if (TargetCell.Row == 0)
                    {
                        return false;
                    }
                    else
                    {
                        returnedCell = Cells[TargetCell.Row - 1, TargetCell.Column];
                        return true;
                    }
                case DragDirection.Right:
                    if (TargetCell.Column == 3)
                    {
                        return false;
                    }
                    else
                    {
                        returnedCell = Cells[TargetCell.Row, TargetCell.Column + 1];
                        return true;
                    }
                case DragDirection.Down:
                    if (TargetCell.Row == 3)
                    {
                        return false;
                    }
                    else
                    {
                        returnedCell = Cells[TargetCell.Row + 1, TargetCell.Column];
                        return true;
                    }
                default:
                    return false;
            }

        }

        /// <summary>
        /// <para>Makes a move and updates the grid accordingly.</para>
        /// <para>Returns true if the game is over after this move.</para>
        /// </summary>
        /// <param name="direction">The direction in which the move was made</param>
        /// <param name="scoreDelta">The score delta generated by the move</param>
        /// <returns>True if the game is over after that move.</returns>
        public bool MakeMove(DragDirection direction, out int scoreDelta)
        {
            scoreDelta = 0;
            bool gameOver = false;
            int numberOfEmptyCells = 0;
            List<Move> Moves = new List<Move>();
            List<PositionOnBoard> ValuesChanged = new List<PositionOnBoard>();
            Cell NewCell = new Cell();

            Moves = MoveAllCells(direction, out ValuesChanged, ref scoreDelta);
            numberOfEmptyCells = CountEmptyCells();
            NewCell = InsertRandomNewCell(numberOfEmptyCells);
            if (MoveMadeOnGrid != null)
                MoveMadeOnGrid(new MoveMadeEventArgs(Moves, ValuesChanged, NewCell));

            SetCanSizeInDirection();

            gameOver = IsGameOver();

            return gameOver;
        }

        public Cell GetLargestCellOnGrid()
        {
            Cell largestCell = new Cell();
            for (int row = 0; row < 4; row++)
            {
                for (int column = 0; column < 4; column++)
                {
                    if (Cells[row, column].CompareTo(largestCell) > 0)
                    {
                        largestCell = Cells[row, column];
                    }
                }
            }

            return largestCell;
        }

        public delegate void MoveMade(MoveMadeEventArgs e);
        public event MoveMade MoveMadeOnGrid;

        /// <summary>
        /// Gets the value of the cell held at the specified row and column positions.
        /// </summary>
        public int this[int row, int column]
        {
            get { return Cells[row, column].Value; }
            set { Cells[row, column].Value = value; }
        }

        /// <summary>
        /// Gets the cell held at the specified board position.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Cell this[PositionOnBoard pos]
        {
            get { return Cells[pos.Row, pos.Column]; }
            set { Cells[pos.Row, pos.Column] = value; }
        }

        internal PositionOnBoard GetBasePosition()
        {
            if (Cells[3, 0].Value != 0)
            {
                return new PositionOnBoard(3, 0);
            }
            else
            {
                List<PositionOnBoard> positionsToCheck = new List<PositionOnBoard>
                {
                    new PositionOnBoard(0, 0),
                    new PositionOnBoard(0, 3),
                    new PositionOnBoard(3, 0),
                    new PositionOnBoard(3, 3)
                };

                PositionOnBoard bestPositionHolder = PositionOnBoard.nullPosition;
                int highestCellHolder = 0;

                foreach (PositionOnBoard position in positionsToCheck)
                {
                    if (this[position].Value > highestCellHolder)
                    {
                        highestCellHolder = this[position].Value;
                        bestPositionHolder = position;
                    }
                }

                return bestPositionHolder;
            }
        }
    }
}
