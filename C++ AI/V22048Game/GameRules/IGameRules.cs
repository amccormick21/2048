using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V22048Game.Elements;

namespace V22048Game.GameRules
{
    /// <summary>
    /// Specifies the methods required to define a type of 2048 game.
    /// </summary>
    public interface IGameRules
    {
        GameValue GetNullValue();
        GameValue GetEmptyCellValue();

        bool CanCombine(GameValue complexNumber, GameValue lastNumericalCellValue);

        int GetScore(GameValue Value);
        NewValuePair GetNewValuePair(Random rand);
        GameValue UpdateValue(GameValue FirstValue, GameValue SecondValue);

        string HighScoreFile { get; }
        string HighBlockFile { get; }
    }
}
