using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PacmanGame.GameStates;
using PacmanGame.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacmanGame.GameStates
{
    class IntroState : GameState
    {
        // Reference to the GameStateManager
        GameStateManager gsm;
        SoundBank soundBank;

        Sprite sprTitle;
        Sprite sprWall;

        float titleScale;

        // Menu stuff
        string[] menuItems = new string[]
        {
            "START",
            "CREDITS",
            "QUIT"
        };

        int mnItemIndex;

        public IntroState(GameStateManager gsm, SoundBank soundBank, Texture2D texAtlas)
        {
            this.gsm = gsm;
            this.soundBank = soundBank;

            sprTitle = new Sprite(texAtlas, new Rectangle(1, 454, 184, 48), new Vector2(92, 24), Color.White);
            sprTitle.Scale = 4;
            sprWall = new Sprite(texAtlas, new Rectangle(0, 32, 32, 32), new Vector2(), Color.White);
            sprWall.Scale = 2;

            soundBank.playTheme();
        }


        public void handleInput(InputProcessor input)
        {
            if (input.isKeyPressed(Keys.Down))
                mnItemIndex++;
            else if (input.isKeyPressed(Keys.Up))
                mnItemIndex--;

            if (mnItemIndex < 0)
                mnItemIndex = menuItems.GetUpperBound(0);
            else if (mnItemIndex > menuItems.GetUpperBound(0))
                mnItemIndex = 0;

            if (input.isKeyPressed(Keys.Enter))
            {
                soundBank.playSfx("blip", 0.3f, false);

                switch(mnItemIndex)
                {
                    case 0: gsm.setState(GameStateManager.States.SELECT_MAP, "0");
                        break;
                    case 1: gsm.setState(GameStateManager.States.CREDITS, "");
                        break;
                    case 2: gsm.exit();
                        break;
                    default:
                        break;
                }

                
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
                FontUtils.drawOutline(sb, Game.fontBig, menuItems[i],
                    new Vector2(Game.WIDTH / 2 - Game.fontBig.MeasureString(menuItems[i]).X / 2, Game.HEIGHT / 2 + 100 + 50 * i),
                    i == mnItemIndex ? Color.White : Color.Gray, Color.Black);
            }

            int posY = 590;
            FontUtils.drawOutline(sb, Game.fontSmall, "CONTROLS:", new Vector2(10, posY), Color.White, Color.Black);
            posY += 20;
            FontUtils.drawOutline(sb, Game.fontSmall, "MOVE - ARROW KEYS", new Vector2(10, posY), Color.White, Color.Black);
            posY += 20;
            FontUtils.drawOutline(sb, Game.fontSmall, "SELECT - ENTER", new Vector2(10, posY), Color.White, Color.Black);
            posY += 20;
            FontUtils.drawOutline(sb, Game.fontSmall, "BACK/PAUSE - ESC", new Vector2(10, posY), Color.White, Color.Black);
            posY += 20;
            FontUtils.drawOutline(sb, Game.fontSmall, "STOP/PLAY SOUND - M", new Vector2(10, posY), Color.White, Color.Black);
            posY += 20;
            FontUtils.drawOutline(sb, Game.fontSmall, "FULLSCREEN - F", new Vector2(10, posY), Color.White, Color.Black);
        }
    }
}
