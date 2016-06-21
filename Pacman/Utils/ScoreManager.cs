using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacmanGame.Utils
{
    class ScoreManager
    {
        int score;

        public void addToScore(int value)
        {
            score += value;
        }

        public void reset()
        {
            score = 0;
        }

        public int Score
        {
            get { return score; }
        }
    }
}
