using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace V22048Game.Gameplay
{
    public class Records
    {
        Dictionary<int, int> HighScores { get; set; }
        Dictionary<int, int> HighestBlocks { get; set; }

        public Records()
        {
            HighScores = new Dictionary<int, int>();
            HighestBlocks = new Dictionary<int, int>();
        }

        internal void Init()
        {
            GetHighScores(GameController.GridSize);
            GetHighestBlocks(GameController.GridSize);
        }

        internal void GetHighScores(int gridSize)
        {
            string[] lineParts;

            try
            {
                using (var sr = new StreamReader(GameController.GameRules.HighScoreFile))
                {
                    while (!sr.EndOfStream)
                    {
                        lineParts = sr.ReadLine().Split(',');
                        HighScores.Add(Convert.ToInt32(lineParts[0]), Convert.ToInt32(lineParts[1]));

                        if (Convert.ToInt32(lineParts[0]) == gridSize) { HighScoreChanged(this, Convert.ToInt32(lineParts[1])); }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                File.Create(GameController.GameRules.HighScoreFile);
            }
        }

        internal void GetHighestBlocks(int gridSize)
        {
            string[] lineParts;

            try
            {
                using (var sr = new StreamReader(GameController.GameRules.HighBlockFile))
                {
                    while (!sr.EndOfStream)
                    {
                        lineParts = sr.ReadLine().Split(',');
                        HighestBlocks.Add(Convert.ToInt32(lineParts[0]), Convert.ToInt32(lineParts[1]));

                        if (Convert.ToInt32(lineParts[0]) == gridSize) { HighestBlockChanged(this, Convert.ToInt32(lineParts[1])); }
                    }
                }
            }
            catch (FileNotFoundException)
            {
                File.Create(GameController.GameRules.HighBlockFile);
            }
        }

        internal void WriteHighScores()
        {
            string keyValuePair;

            using (var sw = new StreamWriter(GameController.GameRules.HighScoreFile))
            {
                foreach (var pair in HighScores)
                {
                    keyValuePair = pair.Key + "," + pair.Value;
                    sw.WriteLine(keyValuePair);
                }
            }
        }

        internal void WriteHighestBlocks()
        {
            string keyValuePair;

            using (var sw = new StreamWriter(GameController.GameRules.HighBlockFile))
            {
                foreach (var pair in HighestBlocks)
                {
                    keyValuePair = pair.Key + "," + pair.Value;
                    sw.WriteLine(keyValuePair);
                }
            }
        }

        internal void SetScore(int score)
        {
            ScoreChanged(this, score);
            SetHighScore(GameController.GridSize, score);
        }

        internal void SetHighBlock(int highBlock)
        {
            HighBlockChanged(this, highBlock);
            SetHighestBlock(GameController.GridSize, highBlock);
        }

        internal void SetHighScore(int gridSize, int currentScore)
        {
            int currentHighScore;
            if (HighScores.TryGetValue(gridSize, out currentHighScore))
            {
                if (currentHighScore < currentScore)
                {
                    HighScores[gridSize] = currentScore;
                    HighScoreChanged(this, currentScore);
                }
            }
            else
            {
                HighScores.Add(gridSize, currentScore);
                HighScoreChanged(this, currentScore);
            }
        }


        internal void SetHighestBlock(int gridSize, int currentHighestBlock)
        {
            int highestBlock;
            if (HighestBlocks.TryGetValue(gridSize, out highestBlock))
            {
                if (highestBlock < currentHighestBlock)
                {
                    HighestBlocks[gridSize] = currentHighestBlock;
                    HighestBlockChanged(this, currentHighestBlock);
                }
            }
            else
            {
                HighestBlocks.Add(gridSize, currentHighestBlock);
                HighestBlockChanged(this, currentHighestBlock);
            }
        }

        internal void GameOver()
        {
            WriteHighScores();
            WriteHighestBlocks();
        }

        internal event EventHandler<int> HighScoreChanged;
        internal event EventHandler<int> HighestBlockChanged;
        internal event EventHandler<int> ScoreChanged;
        internal event EventHandler<int> HighBlockChanged;
    }
}
