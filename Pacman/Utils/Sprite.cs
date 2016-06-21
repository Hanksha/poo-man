using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacmanGame.Utils
{
    /// <summary>
    /// Holds the texture region, position, size and color that can be drawn using SpriteBatch.
    /// </summary>
    class Sprite
    {
        // Reference to the texture atlas
        Texture2D texAtlas;
        // Texture region of the texture atlas
        Rectangle texReg;
        // Center tof the sprite (from top left origin)
        Vector2 center;
        // Color of the sprite
        Color color;
        //Scale of the sprite
        float scale;

        /// <summary>
        /// Creates a sprite.
        /// </summary>
        /// <param name="texAtlas">Reference to the texture atlas</param>
        /// <param name="texRegion">Texture region of the texture atlas</param>
        /// <param name="center">Center of the sprite (from top left origin)</param>
        /// <param name="color">Color of the sprite</param>
        public Sprite(Texture2D texAtlas, Rectangle texRegion, Vector2 center, Color color)
        {
            this.texAtlas = texAtlas;
            texReg = texRegion;
            this.center = center;
            this.color = color;
            scale = 1;
        }

        public Sprite()
        {

        }

        public void draw(SpriteBatch sb, Vector2 pos, bool rot90, bool flipH, bool flipV)
        {
            SpriteEffects effect = SpriteEffects.None;

            if (flipH)
                effect |= SpriteEffects.FlipHorizontally;

            if (flipV)
                effect |= SpriteEffects.FlipVertically;

            float rotAngle = 0;

            if (rot90)
            {
                rotAngle = (float)(Math.PI / 2);

                if (flipH && flipV)
                {
                }
                else if (flipH || flipV)
                {
                    rotAngle += (float)(Math.PI);
                }
            }

            sb.Draw(texAtlas, pos, texReg, color, rotAngle, center, scale, effect, 0);
        }

        public Texture2D Texture
        {
            get
            {
                return texAtlas;
            }
        }
        
        public Rectangle TextureRegion 
        {
            get
            {
                return texReg;
            }

            set
            {
                texReg = value;
            }
        }

        public Vector2 Center
        {
            get
            {
                return center;
            }

            set
            {
                center = value;
            }
        }

        public Color Color
        {
            get
            {
                return color;
            }

            set
            {
                color = value;
            }
        }

        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }
    }
}
