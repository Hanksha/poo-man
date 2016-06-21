using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PacmanGame.GameStates;
using PacmanGame.Maps;
using PacmanGame.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacmanGame.GameStates
{
    class SelectMapState : GameState
    {
        // Reference to the GameStateManager
        GameStateManager gsm;
        SoundBank soundBank;
        Sprite[] sprPreviews;

        Sprite sprTitle;
        Sprite sprWall;

        float titleScale;

        // Menu stuff
        string[] menuItems;

        int mnItemIndex;

        public SelectMapState(GameStateManager gsm, SoundBank soundBank, Texture2D texAtlas, MapManager mapMngr)
        {
            this.gsm = gsm;
            this.soundBank = soundBank;

            sprTitle = new Sprite(texAtlas, new Rectangle(1, 454, 184, 48), new Vector2(92, 24), Color.White);
            sprTitle.Scale = 4;
            sprWall = new Sprite(texAtlas, new Rectangle(0, 32, 32, 32), new Vector2(), Color.Gray);
            sprWall.Scale = 2;

            menuItems = new string[mapMngr.Maps.Length];

            for (int i = 0; i < menuItems.Length; i++)
                menuItems[i] = mapMngr.Maps[i].Name;

            RenderTarget2D[] texMapPreviews = new RenderTarget2D[mapMngr.Maps.Length];
            sprPreviews = new Sprite[mapMngr.Maps.Length];

            for (int i = 0; i < texMapPreviews.Length; i++)
            {
                texMapPreviews[i] = new RenderTarget2D( gsm.GetGame.GraphicsDevice,
                                                        gsm.GetGame.GraphicsDevice.PresentationParameters.BackBufferWidth,
                                                        gsm.GetGame.GraphicsDevice.PresentationParameters.BackBufferHeight);

                gsm.GetGame.GraphicsDevice.SetRenderTarget(texMapPreviews[i]);
                gsm.GetGame.spriteBatch.Begin();
                mapMngr.Maps[i].load();
                mapMngr.Maps[i].draw(gsm.GetGame.spriteBatch);
                gsm.GetGame.spriteBatch.End();

                sprPreviews[i] = new Sprite(texMapPreviews[i],
                    new Rectangle(0, 0, texMapPreviews[i].Width, texMapPreviews[i].Height),
                    new Vector2(texMapPreviews[i].Width / 2, texMapPreviews[i].Height / 2), Color.White);
                sprPreviews[i].Scale = 0.25f;
            }

            gsm.GetGame.GraphicsDevice.SetRenderTarget(null);

            soundBank.playTheme();
        }


        public void handleInput(InputProcessor input)
        {
            if (input.isKeyPressed(Keys.Right))
                mnItemIndex++;
            else if (input.isKeyPressed(Keys.Left))
                mnItemIndex--;

            if (mnItemIndex < 0)
                mnItemIndex = menuItems.GetUpperBound(0);
            else if (mnItemIndex > menuItems.GetUpperBound(0))
                mnItemIndex = 0;

            if (input.isKeyPressed(Keys.Escape))
                gsm.setState(GameStateManager.States.INTRO, "");

            if (input.isKeyPressed(Keys.Enter))
            {
                gsm.setState(GameStateManager.States.PLAY, "" + mnItemIndex);
                soundBank.playSfx("blip", 0.3f, false);
            }
        }

        public void update(double dt)
        {
            titleScale += (float)(120 * dt);

            if (titleScale >= 360)
                titleScale = 0;

            sprTitle.Scale = 4 + 0.2f * (float)Math.Sin(MathHelper.ToRadians(titleScale));
        }

        public void render(SpriteBatch sb)
        {
            gsm.GetGame.modelView.Translation = new Vector3(0, 0, 0);

            for (int row = 0; row < Math.Ceiling(Game.HEIGHT / 64f); row ++)
            {
                for (int col = 0; col < Game.WIDTH / 64; col++)
                {
                    sprWall.draw(sb, new Vector2(col * 64, row * 64), false, false, false);
                }
            }

            sprTitle.draw(sb, new Vector2(Game.WIDTH / 2, 150), false, false, false);


            for(int i = 0; i < menuItems.Length; i++)
            {
               sprPreviews[i].draw(sb, new Vector2(Game.WIDTH / 2 + i * 325 - mnItemIndex * 325, Game.HEIGHT / 2 + 100 + (i != mnItemIndex?50:0)), false, false, false);
                FontUtils.drawOutline(sb,
                     Game.fontBig, menuItems[i], 
                    new Vector2(Game.WIDTH / 2 - Game.fontBig.MeasureString(menuItems[i]).X / 2 + i * 325 - mnItemIndex * 325, Game.HEIGHT / 2 - 50 + (i != mnItemIndex ? 50 : 0)),
                    i == mnItemIndex?Color.White: Color.Gray, Color.Black);
            }
        }
    }
}
