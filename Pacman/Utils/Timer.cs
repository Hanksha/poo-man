using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacmanGame.Utils
{
    class Timer
    {
        int delay;
        int prevTime;
        bool paused;
        int remaining;

        public Timer(int delay)
        {
            this.delay = delay;
            prevTime = Environment.TickCount;
        }

        public bool tick()
        {
            if (paused || delay < 0)
                return false;

            if (delay <= 0)
                return true;
            if (prevTime + delay < Environment.TickCount)
                return true;

            return false;
        }

        public void start()
        {
            prevTime = Environment.TickCount;
        }
    
        public void pause()
        {
            if (paused)
                return;

            paused = true;

            remaining = (prevTime + delay) - Environment.TickCount;
        }

        public void resume()
        {
            if (!paused)
                return;

            paused = false;

            prevTime = remaining + Environment.TickCount - delay;

            remaining = 0;
        }

        public void forceTick()
        {
            prevTime = Environment.TickCount - delay;
        }

        public int Delay
        {
            get { return delay; }

            set
            {
                delay = value;
                start();
            }
        }

        public int PercentRemaining
        {
            get { return (int)((RemainingTime / (delay * 1f)) * 100); }
        }

        public int RemainingTime
        {
            get { return (int) Math.Max(paused?remaining:prevTime + delay - Environment.TickCount, 0); }
        }
    }
}
