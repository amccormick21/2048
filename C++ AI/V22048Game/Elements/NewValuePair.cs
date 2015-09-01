using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V22048Game.GameRules;

namespace V22048Game.Elements
{
    public class NewValuePair
    {
        public GameValue NewValue { get; set; }
        public int Generation { get; set; }

        public NewValuePair(GameValue value, int generation)
        {
            NewValue = value;
            Generation = generation;
        }

        public NewValuePair()
        {
        }
    }
}
