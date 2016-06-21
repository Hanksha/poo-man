using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using PacmanGame.Maps;
using PacmanGame.Entities;
using Microsoft.Xna.Framework.Input;
using PacmanGame.Utils;
using Microsoft.Xna.Framework;

namespace PacmanGame.GameStates
{
    class PlayState : GameState
    {
        GameStateManager gsm;
        SoundBank soundBank;
        GameRuleManager ruleManager;
        Texture2D texAtlas;
        List<FloatingText> texts;

        bool debug, showPauseMenu;
        Random rand = new Random();
        Timer timerScreenShake;

        // Menu stuff
        string[] menuItems = new string[]
        {
            "RESUME",
            "QUIT"
        };

        int mnItemIndex;

        Map map;
        Pacman pacman;
        Ghost[] ghosts;

        public PlayState(GameStateManager gsm, SoundBank soundBank, MapManager mapMngr, string arg, Texture2D texAtlas)
        {
            this.gsm = gsm;
            this.soundBank = soundBank;
            this.texAtlas = texAtlas;

            timerScreenShake = new Timer(500);
            timerScreenShake.forceTick();

            texts = new List<FloatingText>();

            map = mapMngr.Maps[int.Parse(arg)];
            ruleManager = new GameRuleManager(this, texAtlas);

            pacman = new Pacman(texAtlas, this);
            ghosts = new Ghost[4];
            ghosts[0] = new GhostBlinky(texAtlas, this, pacman);
            ghosts[1] = new GhostPinky(texAtlas, this, pacman);
            ghosts[2] = new GhostInky(texAtlas, this, pacman);
            ghosts[3] = new GhostClyde(texAtlas, this, pacman);

            ghosts[0].Home = map.BlinkyHome;
            ghosts[1].Home = map.PinkyHome;
            ghosts[2].Home = map.InkyHome;
            ghosts[3].Home = map.ClydeHome;

            ruleManager.resetAll();

            soundBank.playSfx("intro_theme", 0.2f, false);
        }

        public void handleInput(InputProcessor input)
        {
            if (input.isKeyPressed(Keys.F1))
                debug = !debug;

            if (input.isKeyPressed(Keys.Escape))
            {
                showPauseMenu = !showPauseMenu;
                if (showPauseMenu)
                    ruleManager.pause();
                else
                    ruleManager.resume();
            }

            if (showPauseMenu)
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
                    switch (mnItemIndex)
                    {
                        case 0:
                            ruleManager.resume();
                            showPauseMenu = false;
                            break;
                        case 1:
                            gsm.setState(GameStateManager.States.SELECT_MAP, "0");
                            break;
                        default:
                            break;
                    }
                }
            }

            if (input.isKeyPressed(Keys.Space))
            ruleManager.nextRound();

            pacman.handleInput(input);
        }

        public void update(double dt)
        {
            if (ruleManager.CurrentState == GameRuleManager.State.PLAY)
                soundBank.playTheme();
            else
                soundBank.pauseTheme();


            if (showPauseMenu)
                return;

            pacman.update(dt);

            foreach (Ghost ghost in ghosts)
                ghost.update(dt);

            ruleManager.update(dt);

            for(int i = 0; i < texts.Count; i++)
            {
                if (texts[i].shouldRemove())
                    texts.RemoveAt(i);
            }

            if (ruleManager.isGameOver())
                gsm.setState(GameStateManager.States.SELECT_MAP, "");
        }

        public void render(SpriteBatch sb)
        {
            if(!timerScreenShake.tick())
                gsm.GetGame.modelView.Translation = new Vector3(Game.WIDTH / 2 - map.Width / 2 + rand.Next(10), Game.HEIGHT / 2 - map.Height / 2 + rand.Next(10), 0);
            else
                gsm.GetGame.modelView.Translation = new Vector3(Game.WIDTH / 2 - map.Width / 2, Game.HEIGHT / 2 - map.Height / 2, 0);
            
            map.draw(sb);

            pacman.draw(sb);

            foreach (Ghost ghost in ghosts)
                ghost.draw(sb);

            ruleManager.draw(sb);

            foreach (FloatingText text in texts)
                text.draw(sb);

            if (debug)
                ruleManager.drawDebug(sb);

            if (showPauseMenu)
            {
                FontUtils.drawOutline(sb, Game.fontBig, "PAUSE", new Vector2(Game.WIDTH / 2 - Game.fontBig.MeasureString("PAUSE").X / 2, Game.HEIGHT / 2 + 30), Color.White, Color.Black);

                for (int i = 0; i < menuItems.Length; i++)
                {
                    FontUtils.drawOutline(sb,
                         Game.fontBig, menuItems[i],
                        new Vector2(Game.WIDTH / 2 - Game.fontBig.MeasureString(menuItems[i]).X / 2, Game.HEIGHT / 2 + 100 + 50 * i),
                        i == mnItemIndex ? Color.White : Color.Gray, Color.Black);
                }
            }
        }

        public void resetPositions()
        {
            pacman.setPosition(Tile.toCenterCell(map.PacmanStart.X), Tile.toCenterCell(map.PacmanStart.Y));
            pacman.CurrentMode = Pacman.Mode.NORMAL;

            ghosts[0].setPosition(Tile.toCenterCell(map.BlinkyStart.X), Tile.toCenterCell(map.BlinkyStart.Y));
            ghosts[0].CurrentMode = Ghost.Mode.SCATTER;
            ghosts[1].setPosition(Tile.toCenterCell(map.PinkyStart.X), Tile.toCenterCell(map.PinkyStart.Y));
            ghosts[1].CurrentMode = Ghost.Mode.LEAVE_PEN;
            ghosts[2].setPosition(Tile.toCenterCell(map.InkyStart.X), Tile.toCenterCell(map.InkyStart.Y));
            ghosts[2].CurrentMode = Ghost.Mode.IN_PEN;
            ghosts[3].setPosition(Tile.toCenterCell(map.ClydeStart.X), Tile.toCenterCell(map.ClydeStart.Y));
            ghosts[3].CurrentMode = Ghost.Mode.IN_PEN;
        }

        public void addFloatingText(string text, Vector2 pos, Color color)
        {
            texts.Add(new FloatingText(text, pos, color));
        }

        public void toggleScreenShake()
        {
            timerScreenShake.start();
        }

        public Pacman getPacman()
        {
            return pacman;
        }

        public Ghost[] getGhosts()
        {
            return ghosts;
        }

        public Map getMap()
        {
            return map;
        }

        public SoundBank Sounds
        {
            get { return soundBank; }
        }

        public GameRuleManager RuleManager
        {
            get { return ruleManager; }
        }
    }
}
