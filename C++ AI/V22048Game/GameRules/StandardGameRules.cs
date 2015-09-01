using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V22048Game.Elements;

namespace V22048Game.GameRules
{
    /// <summary>
    /// Provides a set of methods that play the standard rules of the 2048 game.
    /// </summary>
    class StandardGameRules : IGameRules
    {
        public GameValue GetNullValue()
        {
            return new GameValue(-1, 0);
        }

        public GameValue GetEmptyCellValue()
        {
            return GameValue.zero;
        }

        public bool CanCombine(GameValue firstValue, GameValue secondValue)
        {
            return (firstValue.Equals(secondValue));
        }

        public int GetScore(GameValue value)
        {
            return (int)value.Mod;
        }

        public NewValuePair GetNewValuePair(Random rand)
        {
            int randomValue = rand.Next(0, 10);
            if (randomValue == 0)
            {
                return new NewValuePair()
                {
                    NewValue = new GameValue(4, 0),
                    Generation = 2
                };
            }
            else
            {
                return new NewValuePair()
                {
                    NewValue = new GameValue(2, 0),
                    Generation = 1
                };
            }
        }

        public GameValue UpdateValue(GameValue FirstValue, GameValue SecondValue)
        {
            return FirstValue + SecondValue;
        }

        public string HighScoreFile
        {
            get { return "HighScores.txt"; }
        }
        public string HighBlockFile
        {
            get { return "HighBlocks.txt"; }
        }
    }
}
