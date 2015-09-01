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
    public class Cell : IEquatable<Cell>
    {
        public int Row { get; private set; }
        public int Column { get; private set; }
        public GameValue Value { get; private set; }
        public int Generation { get; private set; }

        public Dictionary<Direction, int> NextPosition { get; set; }
        public Dictionary<Direction, MoveAction> NextMoveAction { get; set; }
        public Dictionary<Direction, GameValue> NextMoveMergeValue { get; set; }

        public Cell(int row, int column)
        {
            Row = row;
            Column = column;
            Value = GameController.GameRules.GetEmptyCellValue();
            Generation = 0;
            InitialiseNextMovePositions();
        }

        /// <summary>
        /// Initialises the next position and will double move dictionaries at a value of zero.
        /// </summary>
        private void InitialiseNextMovePositions()
        {
            NextPosition = new Dictionary<Direction, int>();
            NextMoveAction = new Dictionary<Direction, MoveAction>();
            NextMoveMergeValue = new Dictionary<Direction, GameValue>();
            foreach (var direction in Directions.All)
            {
                NextPosition[direction] = -1;
                NextMoveAction[direction] = MoveAction.Constant;
                NextMoveMergeValue[direction] = GameController.GameRules.GetNullValue();
            }
        }

        public Cell(Cell cell)
        {
            Row = cell.Row;
            Column = cell.Column;
            Value = cell.Value;
            Generation = cell.Generation;
            NextPosition = cell.NextPosition;
            NextMoveAction = cell.NextMoveAction;
            NextMoveMergeValue = cell.NextMoveMergeValue;
        }

        public Cell(int row, int column, GameValue value, int generation)
        {
            Row = row;
            Column = column;
            Value = value;
            Generation = generation;
            InitialiseNextMovePositions();
        }

        internal void Shift(Direction direction, int amount)
        {
            if (direction.Level() == Level.Horizontal) { Column += amount; }
            else { Row += amount; }
        }

        private int GetAmountToShift(Direction direction)
        {
            return (NextPosition[direction] - (direction.Level() == Level.Horizontal ? Column : Row));
        }

        internal bool CanShift(Direction direction)
        {
            int amountToShift = GetAmountToShift(direction);
            return (NextPosition[direction] != -1) && (amountToShift != 0) && (Value != GameController.GameRules.GetEmptyCellValue());         
        }

        internal int Update(Direction direction)
        {
            Generation++;
            Value = GameController.GameRules.UpdateValue(Value, NextMoveMergeValue[direction]);
            return (int)GameController.GameRules.GetScore(Value);
        }

        public bool Equals(Cell other)
        {
            return (this.Position.Equals(other.Position));
        }

        public Position Position
        {
            get
            {
                return new Position() { Row = Row, Column = Column };
            }
            set
            {
                Row = value.Row;
                Column = value.Column;
            }
        }

        internal void Initialise(NewValuePair newValue)
        {
            this.Value = newValue.NewValue;
            Generation = newValue.Generation;
        }

        internal void Zero()
        {
            this.Value = GameController.GameRules.GetEmptyCellValue();
            Generation = 0;
        }

        internal Move GetNextMove(Direction direction)
        {
            Move moveToMake = new Move() { StartPosition = Position };
            if (direction.Level() == Level.Horizontal)
            {
                moveToMake.EndPosition = new Position(Row, NextPosition[direction]);
            }
            else
            {
                moveToMake.EndPosition = new Position(NextPosition[direction], Column);
            }

            return moveToMake;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
