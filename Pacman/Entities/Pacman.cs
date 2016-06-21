using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PacmanGame.GameStates;
using PacmanGame.Maps;
using PacmanGame.Utils;
using System.Diagnostics;

namespace PacmanGame.Entities
{
    class Pacman : MovingEntity
    {
        Animation currAnim;
        Animation[] anims;
        Sprite sprDead;

        SoundEffectInstance sfxEngine;

        const int ANIM_MOVE_SIDE = 0, ANIM_MOVE_DOWN = 1, ANIM_MOVE_UP = 2, ANIM_DEAD = 3;
        int currAnimIndex;

        public enum Mode {NORMAL, FRIGHT, DEAD};
        Mode currMode = Mode.NORMAL;

        public Pacman(Texture2D texAtlas, PlayState game) : base(game)
        {
            anims = new Animation[4];

            Rectangle[] eatFramesSide = new Rectangle[3];

            for(int i = 0; i < eatFramesSide.Length; i++)
                eatFramesSide[i] = new Rectangle(334 + 43 * i, 107, 42, 41);

            Rectangle[] eatFramesDown = new Rectangle[3];

            for (int i = 0; i < eatFramesDown.Length; i++)
                eatFramesDown[i] = new Rectangle(334 + 41 * i, 149, 40, 41);

            Rectangle[] eatFramesUp = new Rectangle[3];

            for (int i = 0; i < eatFramesUp.Length; i++)
                eatFramesUp[i] = new Rectangle(334 + 41 * i, 191, 40, 41);

            Rectangle[] deadFrames = new Rectangle[6];
            for (int i = 0; i < deadFrames.Length; i++)
                deadFrames[i] = new Rectangle(240 + 43 * i, 65, 42, 41);

            sprite = new Sprite(texAtlas, eatFramesSide[1], new Vector2(21, 20), Color.White);
            sprDead = new Sprite(texAtlas, deadFrames[1], new Vector2(21, 20), Color.White);


            anims[ANIM_MOVE_SIDE] = new Animation(sprite, eatFramesSide, 100, true);
            anims[ANIM_MOVE_DOWN] = new Animation(sprite, eatFramesDown, 100, true);
            anims[ANIM_MOVE_UP] = new Animation(sprite, eatFramesUp, 100, true);
            anims[ANIM_DEAD] = new Animation(sprDead, deadFrames, 160, false);

            currAnim = anims[ANIM_MOVE_SIDE];

            updateSpeed();

            sfxEngine = game.Sounds.getSoundInstance("engine_loop", 0.2f, true);
        }

        public void handleInput(InputProcessor input)
        {
            if (input.isKeyPressed(Keys.Left))
            {
                reqDir = Direction.LEFT;
            }
            else if (input.isKeyPressed(Keys.Right))
            {
                reqDir = Direction.RIGHT;
            }
            else if (input.isKeyPressed(Keys.Up))
            {
                reqDir = Direction.UP;
            }
            else if (input.isKeyPressed(Keys.Down))
            {
                reqDir = Direction.DOWN;
            }
        }

        public override void update(double dt)
        {
            if (game.RuleManager.currState == GameRuleManager.State.PLAY)
                base.update(dt);

            updateAnim();
        }

        public void updateAnim()
        {
            if (currMode == Mode.NORMAL || currMode == Mode.FRIGHT)
            {
                if ((currDir == Direction.LEFT || currDir == Direction.RIGHT) && currAnimIndex != ANIM_MOVE_SIDE)
                {
                    currAnim = anims[ANIM_MOVE_SIDE];
                    currAnimIndex = ANIM_MOVE_SIDE;
                    currAnim.reset();
                }
                else if (currDir == Direction.DOWN && currAnimIndex != ANIM_MOVE_DOWN)
                {
                    currAnim = anims[ANIM_MOVE_DOWN];
                    currAnimIndex = ANIM_MOVE_DOWN;
                    currAnim.reset();
                }
                else if (currDir == Direction.UP && currAnimIndex != ANIM_MOVE_UP)
                {
                    currAnim = anims[ANIM_MOVE_UP];
                    currAnimIndex = ANIM_MOVE_UP;
                    currAnim.reset();
                }

                if ((!moving && currMode != Mode.DEAD) || game.RuleManager.currState != GameRuleManager.State.PLAY)
                {
                    currAnim.stop();

                    if (sfxEngine.State == SoundState.Playing)
                        sfxEngine.Pause();
                }
                else
                {
                    currAnim.start();

                    if (sfxEngine.State == SoundState.Paused || sfxEngine.State == SoundState.Stopped)
                        sfxEngine.Play();
                }
            }
            else if (currMode == Mode.DEAD)
            {
                if (sfxEngine.State == SoundState.Playing)
                    sfxEngine.Pause();

                if (currAnimIndex != ANIM_DEAD)
                {
                    currAnimIndex = ANIM_DEAD;
                    anims[ANIM_DEAD].reset();
                }

                anims[ANIM_DEAD].update();
            }

            if(currMode != Mode.DEAD)
                currAnim.update();
        }

        protected override void updateSpeed()
        {
            float percent = 1f;

            if(currMode == Mode.NORMAL)
            {
                if (game.getMap().getTileNature(pos) == "normal")
                {
                    percent = GameData.getSpeed(game.RuleManager.Round, GameData.HeaderSpeed.PAC_NORM);
                }
                else if (game.getMap().getTileNature(pos) == "dot" || game.getMap().getTileNature(pos) == "energizer")
                {
                    percent = GameData.getSpeed(game.RuleManager.Round, GameData.HeaderSpeed.PAC_NORM_DOTS);
                }
            }
            else if (currMode == Mode.FRIGHT)
            {
                if (game.getMap().getTileNature(pos) == "normal")
                {
                    percent = GameData.getSpeed(game.RuleManager.Round, GameData.HeaderSpeed.PAC_FRIGHT);
                }
                else if (game.getMap().getTileNature(pos) == "dot" || game.getMap().getTileNature(pos) == "energizer")
                {
                    percent = GameData.getSpeed(game.RuleManager.Round, GameData.HeaderSpeed.PAC_FRIGHT_DOTS);
                }
            }
            
            speed = percent * baseSpeed;
        }

        public override void draw(SpriteBatch sb)
        {
            sprite.draw(sb, pos, false, currDir == Direction.LEFT, false);

            if (currMode == Mode.DEAD)
                sprDead.draw(sb, pos, false, currDir == Direction.LEFT, false);

        }

        protected override void endPath()
        {
            tryTurn(currDir);

            base.endPath();
        }

        public void setMode(Mode mode)
        {
            currMode = mode;
        }

        public Mode CurrentMode
        {
            get { return currMode; }
            set { currMode = value; }
        }
    }
}
