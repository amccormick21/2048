using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V22048Game.Gameplay;
using V22048Game.GameRules;
using V22048Game.MoveInformation;

namespace V22048Game.Elements
{
    public class Board
    {
        public int GridSize { get; private set; }

        public Cell[,] Cells;

        private Random Rand;

        public Board(int gridSize)
        {
            this.GridSize = gridSize;
            InitialiseCells();
        }

        public Board(Board board)
        {
            this.GridSize = board.GridSize;
            this.Rand = board.Rand;
            this.MoveDistancesAnalysed += board.MoveDistancesAnalysed;
            this.RandomFixed += board.RandomFixed;

            InitialiseCells();

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    Cells[row, col] = new Cell(board[row, col]);
                }
            }
            ResetCells();
            AnalyseNextMoveDistances();
        }

        /// <summary>
        /// Adds initial cells to start the game and then analyses the grid ready for the next move.
        /// </summary>
        internal void PrepareGridForStart(int cellsToStart, bool fixRandom)
        {
            Rand = GetRandom(fixRandom);
            RandomFixed(this, fixRandom);

            InsertRandomNewCells(cellsToStart);
            AnalyseNextMoveDistances();
        }

        /// <summary>
        /// <para>Inserts a specified number of cells of either 2 or 4 at random unoccupied locations on the board.</para>
        /// </summary>
        internal List<Cell> InsertRandomNewCells(int cellsToInsert)
        {
            int cellsThatCanBeInserted;
            List<Cell> emptyCells = new List<Cell>();

            int countOfEmptyCells = 0;
            foreach (Cell cell in Cells)
            {
                if (cell.Value.Equals(GameController.GameRules.GetEmptyCellValue())) { emptyCells.Add(cell); ++countOfEmptyCells; }
            }

            cellsThatCanBeInserted = (cellsToInsert > countOfEmptyCells ? countOfEmptyCells : cellsToInsert);

            List<Cell> newCells = new List<Cell>();

            int randomIndex;

            for (int cellsInserted = 0; cellsInserted < cellsThatCanBeInserted; cellsInserted++)
            {
                randomIndex = Rand.Next(0, countOfEmptyCells - cellsInserted);
                emptyCells[randomIndex].Initialise(GameController.GameRules.GetNewValuePair(Rand));
                newCells.Add(emptyCells[randomIndex]);
                emptyCells.RemoveAt(randomIndex);
            }

            return newCells;
        }

        /// <summary>
        /// Analyses the board to produce statistics about the eventual position of all cells after the next move.
        /// </summary>
        public bool AnalyseNextMoveDistances()
        {
            int currentColumnToCheck = 0;
            int currentRowToCheck = 0;
            int startingChannelIndex, finishingChannelIndex, channelStep;

            int lastFreeCell; //The most recently found unoccupied cell.
            int lastNumericalCell; //The most recently found numerical cell.
            Cell currentLastNumericalCell; //The actual cell that was most recently found.
            GameValue lastNumericalCellValue; //The most recently found numerical cell value.

            bool canMove = false; //Remains false unless a move can be made on the grid.

            foreach (Direction direction in Directions.All)
            {
                if ((int)direction >> 1 == 0)
                {
                    startingChannelIndex = -1;
                    finishingChannelIndex = GridSize;
                    channelStep = 1;
                }
                else
                {
                    startingChannelIndex = GridSize;
                    finishingChannelIndex = -1;
                    channelStep = -1; //Always 1 or -1
                }

                for (int i = 0; i < GridSize; i++)
                {
                    //Allocate the major channel to variables.
                    if (direction.Level() == Level.Horizontal) { currentRowToCheck = i; }
                    else { currentColumnToCheck = i; }

                    //Initialise the last cell variables:
                    lastNumericalCell = -1;
                    lastNumericalCellValue = GameController.GameRules.GetNullValue();
                    lastFreeCell = -1;
                    currentLastNumericalCell = null;

                    for (int j = startingChannelIndex + channelStep; j != finishingChannelIndex; j += channelStep)
                    {
                        //Allocate the minor channel to variables
                        if (direction.Level() == Level.Horizontal) { currentColumnToCheck = j; }
                        else { currentRowToCheck = j; }

                        //Scan:
                        if (Cells[currentRowToCheck, currentColumnToCheck].Value.Equals(GameController.GameRules.GetEmptyCellValue()))
                        {
                            if (lastFreeCell == -1)
                            {
                                lastFreeCell = j;
                            }
                        }
                        else
                        {
                            //If the cell has value equal to the last numerical cell value,
                            //insert at the last numerical cell location.
                            if (GameController.GameRules.CanCombine(Cells[currentRowToCheck, currentColumnToCheck].Value, lastNumericalCellValue))
                            {
                                Cells[currentRowToCheck, currentColumnToCheck].NextPosition[direction] = lastNumericalCell;
                                Cells[currentRowToCheck, currentColumnToCheck].NextMoveAction[direction] = MoveAction.Act;
                                Cells[currentRowToCheck, currentColumnToCheck].NextMoveMergeValue[direction] = lastNumericalCellValue;
                                //Set the cell that it replaces to go to zero.
                                currentLastNumericalCell.NextMoveAction[direction] = MoveAction.Zero;

                                lastFreeCell = lastNumericalCell + channelStep;
                                lastNumericalCell = -1;
                                lastNumericalCellValue = GameController.GameRules.GetNullValue();
                                currentLastNumericalCell = null;
                                canMove = true;
                            }
                            else
                            {
                                //The cell has a value but it is not equal to the last cell value.
                                if (lastFreeCell != -1)
                                {
                                    //If the cell can be moved, move it here.
                                    Cells[currentRowToCheck, currentColumnToCheck].NextPosition[direction] = lastFreeCell;
                                    //Prepare for the next cells to be entered.
                                    lastNumericalCell = lastFreeCell;
                                    lastFreeCell += channelStep;
                                    //The grid is not entirely blocked.
                                    canMove = true;
                                }
                                else
                                {
                                    //If the cell cannot be moved.
                                    Cells[currentRowToCheck, currentColumnToCheck].NextPosition[direction] = j;
                                    //Prepare for the next cells to be entered.
                                    lastNumericalCell = j;
                                }

                                //Whether it can be moved or not, set the values of the last numerical cell.
                                lastNumericalCellValue = Cells[currentRowToCheck, currentColumnToCheck].Value;
                                currentLastNumericalCell = Cells[currentRowToCheck, currentColumnToCheck];
                            }
                        }
                    }
                }
            }
            if (MoveDistancesAnalysed != null)
                MoveDistancesAnalysed(this, new EventArgs());

            return canMove;
        }

        internal MoveEventArgs Move(Direction direction, out int scoreDelta, out int highBlock)
        {
            List<Cell> doubledCells;
            List<Move> moves;
            MoveCells(direction, out scoreDelta, out highBlock, out doubledCells, out moves);
            ResetCells();
            return new MoveEventArgs() { Moves = moves, DoubledCells = doubledCells };
        }

        internal void WriteGrid()
        {
            for (int row = 0; row < GridSize; row++)
            {
                for (int column = 0; column < GridSize; column++)
                {
                    Console.Out.Write(Cells[row, column].Value + " ");
                }
                Console.Out.WriteLine();
            }
            Console.Out.WriteLine();
            Console.Out.WriteLine();
        }

        private void ResetCells()
        {
            foreach (Direction direction in Directions.All)
            {
                foreach (Cell cell in Cells)
                {
                    //Initialise the variables of the cell.
                    cell.NextPosition[direction] = -1;
                    cell.NextMoveAction[direction] = MoveAction.Constant;
                    cell.NextMoveMergeValue[direction] = GameController.GameRules.GetNullValue();
                }
            }
        }

        private void MoveCells(Direction direction, out int scoreDelta, out int highBlock, out List<Cell> doubledCells, out List<Move> moves)
        {
            doubledCells = new List<Cell>();
            moves = new List<Move>();
            int scoreGenerated = 0;
            scoreDelta = 0;
            highBlock = 0;
            Cell cell, tempCell;
            Move cellMove;
            int currentColumnToCheck = -1;
            int currentRowToCheck = -1;

            int startingChannelIndex, finishingChannelIndex, channelStep;

            //Uses a similar loop construction as the scanning.
            if ((int)direction >> 1 == 0)
            {
                startingChannelIndex = -1;
                finishingChannelIndex = GridSize;
                channelStep = 1;
            }
            else
            {
                startingChannelIndex = GridSize;
                finishingChannelIndex = -1;
                channelStep = -1; //Always 1 or -1
            }

            for (int i = 0; i < GridSize; i++)
            {
                //Allocate the major channel to variables.
                if (direction.Level() == Level.Horizontal) { currentRowToCheck = i; }
                else { currentColumnToCheck = i; }

                for (int j = startingChannelIndex + channelStep; j != finishingChannelIndex; j += channelStep)
                {
                    //Allocate the minor channel to variables
                    if (direction.Level() == Level.Horizontal) { currentColumnToCheck = j; }
                    else { currentRowToCheck = j; }

                    cell = Cells[currentRowToCheck, currentColumnToCheck];

                    if (cell.CanShift(direction))
                    {
                        cellMove = cell.GetNextMove(direction);
                        //Add the move.
                        moves.Add(cellMove);
                    }

                }
            }

            foreach (Move move in moves)
            {
                //Swap the start and end cells.
                //Set the end cell to zero.
                this[move.StartPosition].Position = move.EndPosition;
                tempCell = this[move.EndPosition];
                tempCell.Zero();
                tempCell.Position = move.StartPosition;
                this[move.EndPosition] = this[move.StartPosition];
                this[move.StartPosition] = tempCell;
            }

            foreach (Cell c in Cells)
            {
                if (c.NextMoveAction[direction] == MoveAction.Act)
                {
                    scoreGenerated = c.Update(direction);
                    doubledCells.Add(c);
                    if (scoreGenerated > highBlock) { highBlock = scoreGenerated; }
                    scoreDelta += scoreGenerated;

                }
            }
        }

        public Board TryMove(Direction direction, out int score, out int highBlock)
        {
            Board board = new Board(this);
            board.Move(direction, out score, out highBlock);

            return board;
        }

        public bool CanMove(Direction direction)
        {
            bool canMove = false;
            foreach (Cell cell in Cells)
            {
                if (cell.CanShift(direction))
                    canMove = true;
            }
            return canMove;
        }

        /// <summary>
        /// Initialises the array of cells to contain a full array of zeroes.
        /// </summary>
        private void InitialiseCells()
        {
            Cells = new Cell[GridSize, GridSize];
            for (int row = 0; row < GridSize; row++)
            {
                for (int column = 0; column < GridSize; column++)
                {
                    Cells[row, column] = new Cell(row, column);
                }
            }
        }

        private Random GetRandom(bool fixRandom)
        {
            if (fixRandom) { return new Random(0); }
            else { return new Random(); }
        }

        internal void SetFixRandom(bool fixRandom)
        {
            Rand = GetRandom(fixRandom);
            RandomFixed(this, fixRandom);
        }

        public Cell this[int row, int column]
        {
            get { return Cells[row, column]; }
            private set { Cells[row, column] = value; }
        }

        public Cell this[Position position]
        {
            get { return Cells[position.Row, position.Column]; }
            private set { Cells[position.Row, position.Column] = value; }
        }

        internal event EventHandler MoveDistancesAnalysed;
        internal event EventHandler<bool> RandomFixed;
    }
}
