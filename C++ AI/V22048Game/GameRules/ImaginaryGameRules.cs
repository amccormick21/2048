using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V22048Game.Elements;

namespace V22048Game.GameRules
{
    class ImaginaryGameRules : IGameRules
    {
        public GameValue GetNullValue()
        {
            return new GameValue(-2, 0);
        }

        public GameValue GetEmptyCellValue()
        {
            return GameValue.zero;
        }

        public bool CanCombine(GameValue currentValue, GameValue newValue)
        {
            return currentValue.Equals(newValue);
        }

        public int GetScore(GameValue Value)
        {
            return (int)Value.Real;
        }

        public NewValuePair GetNewValuePair(Random rand)
        {
            int randomValue = rand.Next(0, 10);
            if (randomValue < 3)
            {
                return new NewValuePair()
                {
                    NewValue = GameValue.i,
                    Generation = 1
                };
            }
            else
            {
                return new NewValuePair()
                {
                    NewValue = new GameValue(2, 0),
                    Generation = 4
                };
            }
        }

        public GameValue UpdateValue(GameValue FirstValue, GameValue SecondValue)
        {
            if (FirstValue.Mod == 1 && FirstValue != GameValue.unit) { return FirstValue.Square(); }
            else { return FirstValue + SecondValue; }
        }

        public string HighScoreFile
        {
            get { return "ImaginaryHighScores.txt"; }
        }
        public string HighBlockFile
        {
            get { return "ImaginaryHighBlocks.txt"; }
        }
    }
}
