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
    class CreditsState : GameState
    {
        // Reference to the GameStateManager
        GameStateManager gsm;
        SoundBank soundBank;

        Sprite sprTitle;
        Sprite sprWall;

        float titleScale;

        public CreditsState(GameStateManager gsm, SoundBank soundBank, Texture2D texAtlas)
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

            if (input.isKeyPressed(Keys.Escape))
            {
                gsm.setState(GameStateManager.States.INTRO, "");
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

            sprTitle.draw(sb, new Vector2(Game.WIDTH / 2, 100), false, false, false);


            int posY = 215;
            Vector2 size = Game.fontSmall.MeasureString("Poo-Man is a Pac-Man game clone.");
            FontUtils.drawOutline(sb, Game.fontSmall, "Poo-Man is a Pac-Man game clone.", new Vector2(Game.WIDTH / 2 - size.X / 2, posY), Color.White, Color.Black);
            posY += 20;
            size = Game.fontSmall.MeasureString("It was developed as final project");
            FontUtils.drawOutline(sb, Game.fontSmall, "It was developed as final project", new Vector2(Game.WIDTH / 2 - size.X / 2, posY), Color.White, Color.Black);
            posY += 20;
            size = Game.fontSmall.MeasureString("for the '6AI' subject of Holy Angel University.");
            FontUtils.drawOutline(sb, Game.fontSmall, "for the '6AI' subject of Holy Angel University.", new Vector2(Game.WIDTH / 2 - size.X / 2, posY), Color.White, Color.Black);

            posY += 40;
            size = Game.fontBig.MeasureString("Programmer:");
            FontUtils.drawOutline(sb, Game.fontBig, "Programmer:", new Vector2(Game.WIDTH / 2 - size.X / 2, posY), Color.White, Color.Black);
            posY += 40;
            size = Game.fontSmall.MeasureString("Vivien Jovet (Caldera Games)");
            FontUtils.drawOutline(sb, Game.fontSmall, "Vivien Jovet (Caldera Games)", new Vector2(Game.WIDTH / 2 - size.X / 2, posY), Color.White, Color.Black);

            posY += 40;
            size = Game.fontBig.MeasureString("2D Artist:");
            FontUtils.drawOutline(sb, Game.fontBig, "2D Artist:", new Vector2(Game.WIDTH / 2 - size.X / 2, posY), Color.White, Color.Black);
            posY += 40;
            size = Game.fontSmall.MeasureString("Yukishi (Caldera Games)");
            FontUtils.drawOutline(sb, Game.fontSmall, "Yukishi (Caldera Games)", new Vector2(Game.WIDTH / 2 - size.X / 2, posY), Color.White, Color.Black);

            posY += 40;
            size = Game.fontBig.MeasureString("Game Designer:");
            FontUtils.drawOutline(sb, Game.fontBig, "Game Designer:", new Vector2(Game.WIDTH / 2 - size.X / 2, posY), Color.White, Color.Black);
            posY += 40;
            size = Game.fontSmall.MeasureString("Toru Iwatani ;)");
            FontUtils.drawOutline(sb, Game.fontSmall, "Toru Iwatani ;)", new Vector2(Game.WIDTH / 2 - size.X / 2, posY), Color.White, Color.Black);

            posY = 530;
            size = Game.fontSmall.MeasureString("Audio:");
            FontUtils.drawOutline(sb, Game.fontSmall, "Audio", new Vector2(Game.WIDTH / 2 - size.X / 2, posY), Color.White, Color.Black);
            posY += 30;
            size = Game.fontSmall.MeasureString("Engine sfx - SoundBible.com");
            FontUtils.drawOutline(sb, Game.fontSmall, "Engine sfx - SoundBible.com", new Vector2(Game.WIDTH / 2 - size.X / 2, posY), Color.White, Color.Black);
            posY += 20;
            size = Game.fontSmall.MeasureString("Round Intro Theme - Jonny Atma");
            FontUtils.drawOutline(sb, Game.fontSmall, "Round Intro Theme - Jonny Atma", new Vector2(Game.WIDTH / 2 - size.X / 2, posY), Color.White, Color.Black);
            posY += 20;
            size = Game.fontSmall.MeasureString("Main Theme - Incompetech.com");
            FontUtils.drawOutline(sb, Game.fontSmall, "Main Theme - Incompetech.com", new Vector2(Game.WIDTH / 2 - size.X / 2, posY), Color.White, Color.Black);
            posY += 20;
            size = Game.fontSmall.MeasureString("...");
            FontUtils.drawOutline(sb, Game.fontSmall, "...", new Vector2(Game.WIDTH / 2 - size.X / 2, posY), Color.White, Color.Black);
            posY += 20;
            size = Game.fontSmall.MeasureString("Everything else is legit.");
            FontUtils.drawOutline(sb, Game.fontSmall, "Everything else is legit.", new Vector2(Game.WIDTH / 2 - size.X / 2, posY), Color.White, Color.Black);

            FontUtils.drawOutline(sb, Game.fontSmall, "Copyright 2015 Caldera Games. All right reserved.", new Vector2(5, 700), Color.White, Color.Black);
        }
    }
}
