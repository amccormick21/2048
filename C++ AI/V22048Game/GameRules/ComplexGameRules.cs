using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V22048Game.Elements;

namespace V22048Game.GameRules
{
    class ComplexGameRules : IGameRules
    {
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
            return currentValue.Mod == newValue.Mod;
        }

        public int GetScore(GameValue Value)
        {
            return (int)Value.Real;
        }

        public NewValuePair GetNewValuePair(Random rand)
        {
            int randomValue = rand.Next(0, 10);
            if (randomValue < 5)
            {
                return new NewValuePair()
                {
                    NewValue = new GameValue(0, 2),
                    Generation = 1
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
            return (FirstValue * SecondValue) * 2 / FirstValue.Mod;
        }

        public string HighScoreFile
        {
            get { return "ComplexHighScores.txt"; }
        }
        public string HighBlockFile
        {
            get { return "ComplexHighBlocks.txt"; }
        }
    }
}
