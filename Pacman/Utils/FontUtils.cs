using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacmanGame.Utils
{
    abstract class FontUtils
    {
        public static void drawOutline(SpriteBatch sb, SpriteFont font, string text, Vector2 pos, Color color, Color outLine)
        {
            pos.X -= 1;
            sb.DrawString(font, text, pos, outLine);
            pos.X += 2;
            sb.DrawString(font, text, pos, outLine);
            pos.X -= 1;
            pos.Y -= 1;
            sb.DrawString(font, text, pos, outLine);
            pos.Y += 2;
            sb.DrawString(font, text, pos, outLine);

            pos.Y -= 1;
            sb.DrawString(font, text, pos, color);
        }
    }
}
