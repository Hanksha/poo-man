using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacmanGame.Utils
{
    class FloatingText
    {
        string text;
        Vector2 pos;
        Timer timer;
        Color color;

        public FloatingText(string text, Vector2 pos, Color color)
        {
            this.text = text;
            this.pos = new Vector2(pos.X, pos.Y);
            this.color = color;
            timer = new Timer(500);
        }

        public void draw(SpriteBatch sb)
        {
            FontUtils.drawOutline(sb, Game.fontSmall, text, pos, color, Color.Black);
        }

        public bool shouldRemove()
        {
            return timer.tick();
        }
    }
}
