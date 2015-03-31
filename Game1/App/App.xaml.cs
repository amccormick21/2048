using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace _2048Game
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static bool GameRunning = false;
        public static int HighScore = 0;
        public static bool FixedRandom = false;
        public static int HighestBlock = 0;
    }
}
