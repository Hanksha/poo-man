using Microsoft.Xna.Framework;
using System;

namespace PacmanGame.Utils
{
    /// <summary>
    /// Holds the texture regions (Rectangle) of an animation
    /// and is controlled by a timer.
    /// </summary>
    class Animation
    {
        // Reference to the sprite
        Sprite sprite;
        // Timer
        Timer timer;
        // Loop or not
        bool loop;
        // Array of texture regions
        Rectangle[] texRegs;
        // Current texture region index
        int index;

        bool started;

        public Animation(Sprite sprite, Rectangle[] texRegions, int delay, bool loop)
        {
            this.sprite = sprite;
            texRegs = texRegions;
            timer = new Timer(delay);
            this.loop = loop;
            
            started = true;
        }

        public void update()
        {
            if(timer.tick())
            {
                timer.start();
                onTick();
            }
        }

        private void onTick()
        {
            if (!started)
                return;

            index++;

            if (index >= texRegs.Length)
            {
                if(loop)
                {
                    index = 0;
                }
                else
                {
                    index = texRegs.Length - 1;
                    started = true;
                }
            }

            sprite.TextureRegion = texRegs[index];
        }

        public void reset()
        {
            index = 0;
            sprite.TextureRegion = texRegs[index];
            timer.start();
        }

        public void start()
        {
            started = true;
        }

        public void stop()
        {
            started = false;
        }

        public bool isPlaying()
        {
            return index != texRegs.Length && !timer.tick();
        }

        public bool isStart()
        {
            return started;
        }
    }
}
