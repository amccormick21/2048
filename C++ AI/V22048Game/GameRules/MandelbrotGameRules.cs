using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V22048Game.Elements;

namespace V22048Game.GameRules
{
    class MandelbrotGameRules : IGameRules
    {
        Random rand;
        GameValue firstValue;

        public MandelbrotGameRules()
        {
            rand = new Random();
            firstValue = new GameValue()
            {
                Real = (rand.NextDouble() * 2.5) - 2,
                Imaginary = (rand.NextDouble() * 2) - 1
            };
        }

        public GameValue GetNullValue()
        {
            return new GameValue(-1, 0);
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
            int scoreToReturn = 0;
            if (Value.Mod > 2)
            {
                scoreToReturn = (int)Value.Mod;
            }
            return scoreToReturn;
        }

        public NewValuePair GetNewValuePair(Random rand)
        {
            int randomValue = rand.Next(0, 10);
            if (randomValue == 0)
            {
                return new NewValuePair()
                {
                    NewValue = firstValue.Square() + firstValue,
                    Generation = 2
                };
            }
            else
            {
                return new NewValuePair()
                {
                    NewValue = firstValue,
                    Generation = 1
                };
            }
        }

        public GameValue UpdateValue(GameValue FirstValue, GameValue SecondValue)
        {
            return (FirstValue * SecondValue) + FirstValue;
        }

        public string HighScoreFile
        {
            get { return "MandelbrotHighScores.txt"; }
        }
        public string HighBlockFile
        {
            get { return "MandelbrotHighBlocks.txt"; }
        }

    }
}
