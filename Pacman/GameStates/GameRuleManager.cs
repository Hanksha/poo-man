using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PacmanGame.Entities;
using PacmanGame.Maps;
using PacmanGame.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace PacmanGame.GameStates
{
    class GameRuleManager
    {
        // Reference to the texture atlas
        Texture2D texAtlas;
        Sprite sprLife;
        Sprite sprBonus;

        PlayState game;

        public enum State { START, PLAY, WIN, PACDEAD, GAMEOVER}
        public State currState;
        
        // Number of round on the current level map
        int roundCounter;
        int lifeCounter;
        const int MAX_LIFE = 2;
        int remainDots;
        int scoreMul;

        ScoreManager score;

        // Timers
        Timer timerStart;
        Timer timerDead;
        Timer timerFright;
        Timer timerMode;
        Timer timerPen;
        Timer timerBonus;
        Timer timerGameOver;
        Timer timerWin;
        /// <summary>
        /// true = scatter mode, false = chase mode
        /// </summary>
        bool scatter;
        int modeSwitchCounter;

        public GameRuleManager(PlayState game, Texture2D texAtlas)
        {
            this.game = game;
            score = new ScoreManager();

            sprLife = new Sprite(texAtlas, new Rectangle(463, 107, 41, 41), new Vector2(), Color.White);
            sprBonus = new Sprite(texAtlas, new Rectangle(), new Vector2(), Color.White);

            timerFright = new Timer(7000);
            timerFright.forceTick();
            timerDead = new Timer(3000);
            timerDead.forceTick();
            timerStart = new Timer(3000);
            timerStart.forceTick();
            timerMode = new Timer(0);
            timerMode.forceTick();
            timerPen = new Timer(5000);
            timerPen.forceTick();
            timerBonus = new Timer(15000);
            timerBonus.forceTick();
            timerGameOver = new Timer(5000);
            timerGameOver.forceTick();
            timerWin = new Timer(5000);
            timerWin.forceTick();
        }

        public void resetAll()
        {
            roundCounter = 1;
            lifeCounter = MAX_LIFE;
            resetMap();
            resetRound();

            currState = State.START;
            timerStart.start();
        }

        public void resetRound()
        {
            scoreMul = 1;
            modeSwitchCounter = 0;
            scatter = true;
            game.getMap().setTile(0, Tile.toCell(game.getMap().BonusPosition.Y), Tile.toCell(game.getMap().BonusPosition.X));
            game.resetPositions();
            timerMode.Delay = GameData.getModeTime(roundCounter, modeSwitchCounter);
            timerFright.Delay = GameData.getModeTime(roundCounter, 8);
            timerFright.forceTick();
            timerMode.forceTick();
            timerBonus.forceTick();
        }

        public void resetMap()
        {
            game.getMap().load();
            remainDots = game.getMap().DotCount;
        }

        public void nextRound()
        {
            roundCounter++;
            resetMap();
            resetRound();
            currState = State.START;
            timerStart.start();
        }

        public void update(double dt)
        {
            if(currState == State.START)
            {
                if(timerStart.tick())
                {
                    currState = State.PLAY;
                    timerMode.start();
                    timerPen.start();
                }
            }
            else if(currState == State.PLAY)
            {
                if (timerMode.tick())
                {
                    scatter = !scatter;

                    if(scatter)
                        game.Sounds.playSfx("dog_woof", 0.5f, false);
                    else
                        game.Sounds.playSfx("dog_grr", 0.5f, false);

                    modeSwitchCounter++;

                    timerMode.Delay = GameData.getModeTime(roundCounter, modeSwitchCounter);
                    timerMode.start();

                    foreach (Ghost ghost in game.getGhosts())
                    {
                        ghost.reverseDir();
                    }
                }

                foreach (Ghost ghost in game.getGhosts())
                {
                    if(ghost.CurrentMode == Ghost.Mode.SCATTER || ghost.CurrentMode == Ghost.Mode.CHASE)
                    {
                        ghost.CurrentMode = scatter ? Ghost.Mode.SCATTER : Ghost.Mode.CHASE;
                    }

                    if(ghost.CurrentMode == Ghost.Mode.IN_PEN && timerPen.tick())
                    {
                        timerPen.start();
                        ghost.CurrentMode = Ghost.Mode.LEAVE_PEN;
                    }

                    if(ghost.CurrentCell == game.getPacman().CurrentCell && ghost.CurrentMode != Ghost.Mode.DEAD)
                    {
                        if(ghost.CurrentMode == Ghost.Mode.FRIGHT)
                        {
                            ghost.CurrentMode = Ghost.Mode.DEAD;
                            game.toggleScreenShake();
                            score.addToScore((int)Math.Pow(2, scoreMul) * 100);
                            game.addFloatingText("" + ((int)Math.Pow(2, scoreMul) * 100), game.getPacman().getPosition(), Color.White);
                            if(scoreMul == 4)
                            {
                                score.addToScore(12000);
                                game.addFloatingText("BONUS " + (12000), new Vector2(game.getPacman().getPosition().X, game.getPacman().getPosition().Y - 15), Color.Yellow);
                                scoreMul = 0;
                            }
                            scoreMul++;

                            game.Sounds.playSfx("dog_whine", 0.5f, false);

                            break;
                        }
                        else
                        {
                            game.getPacman().CurrentMode = Pacman.Mode.DEAD;
                            lifeCounter--;
                            game.Sounds.playSfx("explo", 0.5f, false);
                            game.toggleScreenShake();

                            game.Sounds.playSfx("line_lose", 0.5f, false);

                            if (lifeCounter < 0)
                            {
                                currState = State.GAMEOVER;
                                lifeCounter = 0;
                                timerGameOver.start();
                            }
                            else
                            {
                                currState = State.PACDEAD;
                                timerDead.start();
                            }

                            break;
                        }
                    }
                }

                if (timerPen.tick())
                {
                    timerPen.start();
                }

                if (game.getMap().getTileNature(game.getPacman().getPosition()) == "dot")
                {
                    game.getMap().setTile(0, game.getPacman().CurrentCell.Y, game.getPacman().CurrentCell.X);
                    score.addToScore(10);
                    remainDots--;
                    game.Sounds.playSfxRandom("eat", 1, 3, 0.3f, false);
                }
                else if (game.getMap().getTileNature(game.getPacman().getPosition()) == "energizer")
                {
                    game.getMap().setTile(0, game.getPacman().CurrentCell.Y, game.getPacman().CurrentCell.X);
                    score.addToScore(50);
                    timerFright.start();
                    remainDots--;
                    game.Sounds.playSfx("energizer", 0.4f, false);
                }
                else if (game.getMap().getTileNature(game.getPacman().getPosition()) == "bonus")
                {
                    game.getMap().setTile(0, game.getPacman().CurrentCell.Y, game.getPacman().CurrentCell.X);
                    score.addToScore(GameData.getBonusPoint(roundCounter));
                    game.addFloatingText("" + GameData.getBonusPoint(roundCounter), game.getPacman().getPosition(), Color.Blue);
                    timerBonus.forceTick();
                    game.Sounds.playSfx("bonus", 0.4f, false);
                }

                if ((remainDots == 170 || remainDots == 70) && timerBonus.tick())
                {
                    game.getMap().setTile(GameData.getBonusTileId(roundCounter), Tile.toCell(game.getMap().BonusPosition.Y), Tile.toCell(game.getMap().BonusPosition.X));
                    game.Sounds.playSfx("bonus", 0.4f, false);
                    timerBonus.start();
                }

                if (timerBonus.tick())
                {
                    game.getMap().setTile(0, Tile.toCell(game.getMap().BonusPosition.Y), Tile.toCell(game.getMap().BonusPosition.X));
                }

                if (!timerFright.tick() && game.getPacman().CurrentMode != Pacman.Mode.FRIGHT)
                {
                    timerMode.pause();
                    game.getPacman().CurrentMode = Pacman.Mode.FRIGHT;
                    foreach (Ghost ghost in game.getGhosts())
                    {
                        if(ghost.CurrentMode != Ghost.Mode.DEAD)
                        {
                            ghost.CurrentMode = Ghost.Mode.FRIGHT;
                            ghost.reverseDir();
                        }
                    }

                }
                else if (timerFright.tick() && game.getPacman().CurrentMode == Pacman.Mode.FRIGHT)
                {
                    timerMode.resume();
                    game.getPacman().CurrentMode = Pacman.Mode.NORMAL;
                    foreach (Ghost ghost in game.getGhosts())
                    {
                        if (ghost.CurrentMode == Ghost.Mode.FRIGHT)
                        {
                            ghost.CurrentMode = ghost.PreviousMode;
                        }
                    }

                    scoreMul = 1;
                }

                if (remainDots == 0)
                {
                    game.Sounds.playSfx("line_win", 0.5f, false);
                    timerWin.start();
                    currState = State.WIN;
                }
            }
            else if(currState == State.PACDEAD)
            {
                if (timerDead.tick())
                {
                    currState = State.START;
                    timerStart.start();
                    resetRound();
                }
            }
            else if(currState == State.GAMEOVER)
            {
            }
            else if (currState == State.WIN)
            {
                if (timerWin.tick())
                {
                    nextRound();
                }
            }
        }

        public void draw(SpriteBatch sb)
        {
            Vector2 size = Game.fontSmall.MeasureString("Score");
            FontUtils.drawOutline(sb, Game.fontSmall, "Score", new Vector2(640 - size.X / 2, 10), Color.White, Color.Black);

            size = Game.fontSmall.MeasureString("" + score.Score);
            FontUtils.drawOutline(sb, Game.fontSmall, "" + score.Score, new Vector2(640 - size.X / 2, 30), Color.White, Color.Black);

            size = Game.fontSmall.MeasureString("Round");
            FontUtils.drawOutline(sb, Game.fontSmall, "Round", new Vector2(640 - size.X / 2, 50), Color.White, Color.Black);

            size = Game.fontSmall.MeasureString("" + roundCounter);
            FontUtils.drawOutline(sb, Game.fontSmall, "" + roundCounter, new Vector2(640 - size.X / 2, 70), Color.White, Color.Black);

            if (currState == State.GAMEOVER)
            {
                size = Game.fontBig.MeasureString("GAME OVER");
                FontUtils.drawOutline(sb, Game.fontBig, "GAME OVER", new Vector2(640 - size.X / 2, 360 - size.Y / 2), Color.Red, Color.Black);
            }
            else if(currState == State.START)
            {
                size = Game.fontBig.MeasureString("READY!");
                FontUtils.drawOutline(sb, Game.fontBig, "READY!", new Vector2(640 - size.X / 2, 360 - size.Y / 2), Color.Yellow, Color.Black);
            }
            else if (currState == State.WIN)
            {
                size = Game.fontBig.MeasureString("GOOD JOB POO-MAN!");
                FontUtils.drawOutline(sb, Game.fontBig, "GOOD JOB POO-MAN!", new Vector2(640 - size.X / 2, 360 - size.Y / 2), Color.Yellow, Color.Black);
                size = Game.fontBig.MeasureString("NEXT ROUND!");
                FontUtils.drawOutline(sb, Game.fontBig, "NEXT ROUND!", new Vector2(640 - size.X / 2, 400 - size.Y / 2), Color.Yellow, Color.Black);
            }
            else if (currState == State.PACDEAD)
            {
                size = Game.fontBig.MeasureString("ONE MORE CHANCE POO-MAN!");
                FontUtils.drawOutline(sb, Game.fontBig, "ONE MORE CHANCE POO-MAN!", new Vector2(640 - size.X / 2, 360 - size.Y / 2), Color.Yellow, Color.Black);
            }


            for (int i = 0; i < lifeCounter; i++)
            {
                sprLife.draw(sb, new Vector2(550 + 44 * i, 614), false, false, false);
            }

            for (int round = Math.Max(1, roundCounter - 4), i = 0; round <= roundCounter; round++, i++)
            {
                sprBonus.TextureRegion = game.getMap().Tiles.TileRegions[GameData.getBonusTileId(round)];
                sprBonus.draw(sb, new Vector2(550 + 34 * i, 660), false, false, false);
            }
        }

        public void drawDebug(SpriteBatch sb)
        {
            int posY = 0;
            sb.DrawString(Game.fontSmall, "Speed: " + game.getPacman().Speed, new Vector2(0, posY), Color.White);
            posY += 20;
            sb.DrawString(Game.fontSmall, "Mode: " + game.getPacman().CurrentMode, new Vector2(0, posY), Color.White);
            posY += 20;
            sb.DrawString(Game.fontSmall, "Life: " + lifeCounter, new Vector2(0, posY), Color.White);
            posY += 20;
            sb.DrawString(Game.fontSmall, "Score Mul.: " + scoreMul, new Vector2(0, posY), Color.White);
            posY += 20;
            sb.DrawString(Game.fontSmall, "Round State: " + currState.ToString(), new Vector2(0, posY), Color.White);
            posY += 20;
            sb.DrawString(Game.fontSmall, "Dot Left: " + remainDots, new Vector2(0, posY), Color.White);
            posY += 20;
            sb.DrawString(Game.fontSmall, "Dot Left %: " + DotsLeftPercent, new Vector2(0, posY), Color.White);
            posY += 20;
            sb.DrawString(Game.fontSmall, "Timer Mode: " + timerMode.PercentRemaining, new Vector2(0, posY), Color.White);
            posY += 20;
            sb.DrawString(Game.fontSmall, "Timer Fright: " + timerFright.PercentRemaining, new Vector2(0, posY), Color.White);
            posY += 20;
            sb.DrawString(Game.fontSmall, "Timer Pen: " + timerPen.PercentRemaining, new Vector2(0, posY), Color.White);

            posY += 20;
            sb.DrawString(Game.fontSmall, "Blinky Mode: " + game.getGhosts()[0].CurrentMode, new Vector2(0, posY), Color.White);
            posY += 20;
            sb.DrawString(Game.fontSmall, "Pinky Mode: " + game.getGhosts()[1].CurrentMode, new Vector2(0, posY), Color.White);
            posY += 20;
            sb.DrawString(Game.fontSmall, "Inky Mode: " + game.getGhosts()[2].CurrentMode, new Vector2(0, posY), Color.White);
            posY += 20;
            sb.DrawString(Game.fontSmall, "Clyde Mode: " + game.getGhosts()[3].CurrentMode, new Vector2(0, posY), Color.White);
            posY += 20;
            sb.DrawString(Game.fontSmall, "Blinky Speed: " + game.getGhosts()[0].Speed, new Vector2(0, posY), Color.White);
            posY += 20;
            sb.DrawString(Game.fontSmall, "Pinky Speed: " + game.getGhosts()[1].Speed, new Vector2(0, posY), Color.White);
            posY += 20;
            sb.DrawString(Game.fontSmall, "Inky Speed: " + game.getGhosts()[2].Speed, new Vector2(0, posY), Color.White);
            posY += 20;
            sb.DrawString(Game.fontSmall, "Clyde Speed: " + game.getGhosts()[3].Speed, new Vector2(0, posY), Color.White);
        }

        public void pause()
        {
            timerStart.pause();
            timerDead.pause();
            timerFright.pause();
            timerMode.pause();
            timerPen.pause();
            timerBonus.pause();
            timerGameOver.pause();
            timerWin.pause();
        }

        public void resume()
        {
            timerStart.resume();
            timerDead.resume();
            timerFright.resume();
            timerMode.resume();
            timerPen.resume();
            timerBonus.resume();
            timerGameOver.resume();
            timerWin.resume();
        }

        public int Round
        {
            get {return roundCounter;}
        }

        public bool isGameOver()
        {
            return currState == State.GAMEOVER && timerGameOver.tick();
        }

        public State CurrentState
        {
            get { return currState;}
        }

        public Timer TimerMode
        {
            get { return timerMode; }
        }


        public Timer TimerFright
        {
            get { return timerFright; }
        }

        public int DotsLeft
        {
            get { return remainDots; }
        }
        public int DotsLeftPercent
        {
            get { return (int)((remainDots / (game.getMap().DotCount * 1f)) * 100);  }
        }
    }
}
