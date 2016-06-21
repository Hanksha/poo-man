using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PacmanGame.GameStates;
using PacmanGame.Maps;
using PacmanGame.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PacmanGame.Entities
{
    class Ghost : MovingEntity, IComparer<Ghost.VecDir>
    {     
        protected Texture2D texAtlas;

        protected Animation currAnim;
        protected Animation[] anims;
        protected const int ANIM_MOVE_SIDE = 0,
                            ANIM_MOVE_DOWN = 1,
                            ANIM_MOVE_UP = 2,
                            ANIM_FRIGHT_SIDE = 3,
                            ANIM_FRIGHT_END_SIDE = 4,
                            ANIM_FRIGHT_DOWN = 5,
                            ANIM_FRIGHT_UP = 6,
                            ANIM_FRIGHT_END_DOWN = 7,
                            ANIM_FRIGHT_END_UP = 8,
                            ANIM_MOVE_SIDE_CHASE = 9,
                            ANIM_MOVE_DOWN_CHASE = 10;

        protected int currAnimIndex = -1;
        protected Sprite[] sprEyes;

        public enum Mode {IN_PEN, LEAVE_PEN, SCATTER, CHASE, FRIGHT, DEAD};
        protected Mode prevMode;
        protected Mode currMode;

        protected Pacman pacman;

        protected Vector2 home;

        internal class VecDir
        {
            public Vector2 v;
            public Direction dir;

            public VecDir(Vector2 v, Direction dir)
            {
                this.v = v;
                this.dir = dir;
            }
        }

        public Ghost(Texture2D texAtlas, PlayState game, Pacman pacman) : base(game)
        {
            this.texAtlas = texAtlas;
            this.pacman = pacman;

            sprite = new Sprite(texAtlas, new Rectangle(), new Vector2(18, 18), Color.White);

            anims = new Animation[11];

            Rectangle[] frightFramesSide = new Rectangle[3];
            for(int i = 0; i < frightFramesSide.Length; i++)
                frightFramesSide[i] = new Rectangle(223 + 37 * i, 183, 36, 37);

            Rectangle[] frightFramesUp = new Rectangle[3];
            for (int i = 0; i < frightFramesUp.Length; i++)
                frightFramesUp[i] = new Rectangle(223 + 37 * i, 145, 36, 37);

            Rectangle[] frightFramesDown = new Rectangle[3];
            for (int i = 0; i < frightFramesDown.Length; i++)
                frightFramesDown[i] = new Rectangle(223 + 37 * i, 107, 36, 37);

            Rectangle[] frightEndFramesSide = new Rectangle[6];
            for (int i = 0; i < frightEndFramesSide.Length / 2; i++)
            {
                frightEndFramesSide[i] = new Rectangle(223 + 37 * i, 297, 36, 37);
                frightEndFramesSide[i + 3] = new Rectangle(223 + 37 * i, 183, 36, 37);
            }

            Rectangle[] frightEndFramesUp = new Rectangle[6];
            for (int i = 0; i < frightEndFramesUp.Length / 2; i++)
            {
                frightEndFramesUp[i] = new Rectangle(223 + 37 * i, 259, 36, 37);
                frightEndFramesUp[i + 3] = new Rectangle(223 + 37 * i, 145, 36, 37);
            }

            Rectangle[] frightEndFramesDown = new Rectangle[6];
            for (int i = 0; i < frightEndFramesDown.Length / 2; i++)
            {
                frightEndFramesDown[i] = new Rectangle(223 + 37 * i, 221, 36, 37);
                frightEndFramesDown[i + 3] = new Rectangle(223 + 37 * i, 107, 36, 37);
            }

            anims[ANIM_FRIGHT_SIDE] = new Animation(sprite, frightFramesSide, 100, true);
            anims[ANIM_FRIGHT_UP] = new Animation(sprite, frightFramesUp, 100, true);
            anims[ANIM_FRIGHT_DOWN] = new Animation(sprite, frightFramesDown, 100, true);
            anims[ANIM_FRIGHT_END_SIDE] = new Animation(sprite, frightEndFramesSide, 100, true);
            anims[ANIM_FRIGHT_END_UP] = new Animation(sprite, frightEndFramesUp, 100, true);
            anims[ANIM_FRIGHT_END_DOWN] = new Animation(sprite, frightEndFramesDown, 100, true);

            sprEyes = new Sprite[4];
            for (int i = 0; i < sprEyes.Length; i++)
            {
                sprEyes[i] = new Sprite(texAtlas, new Rectangle(7 + 17 * i, 445, 16, 8), new Vector2(), Color.White);
            }

            CurrentMode = Mode.SCATTER;

            currDir = Direction.LEFT;
            reqDir = Direction.NONE;
        }

        public override void update(double dt)
        {

            if(game.RuleManager.currState == GameRuleManager.State.PLAY)
                base.update(dt);

            if(currMode == Mode.FRIGHT)
            {
                if(game.RuleManager.TimerFright.PercentRemaining <= 30)
                {
                    if ((currDir == Direction.LEFT || currDir == Direction.RIGHT) && currAnimIndex != ANIM_FRIGHT_END_SIDE)
                    {
                        currAnimIndex = ANIM_FRIGHT_END_SIDE;
                        currAnim = anims[currAnimIndex];
                        currAnim.reset();
                    }
                    else if (currDir == Direction.DOWN && currAnimIndex != ANIM_FRIGHT_END_DOWN)
                    {
                        currAnimIndex = ANIM_FRIGHT_END_DOWN;
                        currAnim = anims[currAnimIndex];
                        currAnim.reset();
                    }
                    else if (currDir == Direction.UP && currAnimIndex != ANIM_FRIGHT_END_UP)
                    {
                        currAnimIndex = ANIM_FRIGHT_END_UP;
                        currAnim = anims[currAnimIndex];
                        currAnim.reset();
                    }
                }
                else
                {
                    if ((currDir == Direction.LEFT || currDir == Direction.RIGHT) && currAnimIndex != ANIM_FRIGHT_SIDE)
                    {
                        currAnimIndex = ANIM_FRIGHT_SIDE;
                        currAnim = anims[currAnimIndex];
                        currAnim.reset();
                    }
                    else if (currDir == Direction.DOWN && currAnimIndex != ANIM_FRIGHT_DOWN)
                    {
                        currAnimIndex = ANIM_FRIGHT_DOWN;
                        currAnim = anims[currAnimIndex];
                        currAnim.reset();
                    }
                    else if (currDir == Direction.UP && currAnimIndex != ANIM_FRIGHT_UP)
                    {
                        currAnimIndex = ANIM_FRIGHT_UP;
                        currAnim = anims[currAnimIndex];
                        currAnim.reset();
                    }
                }
            }
            else if(currMode == Mode.CHASE)
            {
                if ((currDir == Direction.LEFT || currDir == Direction.RIGHT) && currAnimIndex != ANIM_MOVE_SIDE_CHASE)
                {
                    currAnim = anims[ANIM_MOVE_SIDE_CHASE];
                    currAnimIndex = ANIM_MOVE_SIDE_CHASE;
                    currAnim.reset();
                }
                else if (currDir == Direction.DOWN && currAnimIndex != ANIM_MOVE_DOWN_CHASE)
                {
                    currAnim = anims[ANIM_MOVE_DOWN_CHASE];
                    currAnimIndex = ANIM_MOVE_DOWN_CHASE;
                    currAnim.reset();
                }
                else if (currDir == Direction.UP && currAnimIndex != ANIM_MOVE_UP)
                {
                    currAnim = anims[ANIM_MOVE_UP];
                    currAnimIndex = ANIM_MOVE_UP;
                    currAnim.reset();
                }
            }
            else
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
            }

            currAnim.update();
        }

        private bool canTurn(Direction dir)
        {
            if (currDir == Direction.RIGHT && dir == Direction.LEFT)
                return false;
            if (currDir == Direction.LEFT && dir == Direction.RIGHT)
                return false;
            if (currDir == Direction.UP && dir == Direction.DOWN)
                return false;
            if (currDir == Direction.DOWN && dir == Direction.UP)
                return false;

            return true;
        }

        public void reverseDir()
        {
            if (currMode != Mode.DEAD && currMode != Mode.IN_PEN && currMode != Mode.LEAVE_PEN)
            {
                if (currDir == Direction.LEFT)
                    reqDir = Direction.RIGHT;
                else if (currDir == Direction.RIGHT)
                    reqDir = Direction.LEFT;
                else if (currDir == Direction.UP)
                    reqDir = Direction.DOWN;
                else if (currDir == Direction.DOWN)
                    reqDir = Direction.UP;
            }
        }

        private List<VecDir> getAvailableMoves()
        {
            List<VecDir> list = new List<VecDir>();

            // Left
            if(canTurn(Direction.LEFT) && !isBlock(pos.X - 17, pos.Y))
            {
                list.Add(new VecDir(new Vector2(Tile.toCenterCell(pos.X - 17), Tile.toCenterCell(pos.Y)), Direction.LEFT));
            }
            // Right
            if (canTurn(Direction.RIGHT) && !isBlock(pos.X + 17, pos.Y))
            {
                list.Add(new VecDir(new Vector2(Tile.toCenterCell(pos.X + 17), Tile.toCenterCell(pos.Y)), Direction.RIGHT));
            }
            // UP
            if (canTurn(Direction.UP) && !isBlock(pos.X, pos.Y - 17))
            {
                list.Add(new VecDir(new Vector2(Tile.toCenterCell(pos.X), Tile.toCenterCell(pos.Y - 17)), Direction.UP));
            }
            // Down
            if (canTurn(Direction.DOWN) && !isBlock(pos.X, pos.Y + 17))
            {
                list.Add(new VecDir(new Vector2(Tile.toCenterCell(pos.X), Tile.toCenterCell(pos.Y + 17)), Direction.DOWN));
            }

            return list;
        }

        private void moveDecision()
        {
            if (currMode == Mode.LEAVE_PEN && 
                Tile.toCell(pos.X) == Tile.toCell(game.getMap().PenOut.X) &&
                Tile.toCell(pos.Y) == Tile.toCell(game.getMap().PenOut.Y))
            {
                currMode = Mode.SCATTER;
            }
            else if(currMode == Mode.DEAD &&
               Tile.toCell(pos.X) == Tile.toCell(game.getMap().PenIn.X) &&
               Tile.toCell(pos.Y) == Tile.toCell(game.getMap().PenIn.Y))
            {
                currMode = Mode.IN_PEN;
            }

            List<VecDir> list = getAvailableMoves();
            
            if (currMode == Mode.FRIGHT || currMode == Mode.IN_PEN)
            {
                Random rand = new Random();

                int i = rand.Next(list.Count);

                reqDir = list[i].dir;
            }
            else
            {
                list.Sort(this);

                if (list.Count != 0)
                {
                    reqDir = list[0].dir;
                }
            }
        }

        protected override void endPath()
        {
            moveDecision();

            base.endPath();
        }

        protected override bool isBlock(float x, float y)
        {
            int row = Tile.toCell(y);
            int col = Tile.toCell(x);

            if (row < 0 || row >= game.getMap().NumRow)
                return false;
            if (col < 0 || col >= game.getMap().NumCol)
                return false;

            if ((currMode == Mode.LEAVE_PEN || currMode == Mode.DEAD) && game.getMap().Tiles.MetaData[game.getMap().Grid[row, col].id].nature == "pen_gate")
                return false;

            return game.getMap().Tiles.MetaData[game.getMap().Grid[row, col].id].type == "block";
        }

        protected override void updateSpeed()
        {
            float percent = 1;

            if(currMode == Mode.FRIGHT)
            {
                percent = GameData.getSpeed(game.RuleManager.Round, GameData.HeaderSpeed.GHOST_FRIGHT);
            }
            else if(currMode == Mode.IN_PEN || currMode == Mode.LEAVE_PEN)
            {
                percent = GameData.getSpeed(game.RuleManager.Round, GameData.HeaderSpeed.GHOST_PEN);
            }
            else if(game.getMap().getTileNature(pos) == "tunnel")
            {
                percent = GameData.getSpeed(game.RuleManager.Round, GameData.HeaderSpeed.GHOST_TUNNEL);
            }
            else
            {
                percent = GameData.getSpeed(game.RuleManager.Round, GameData.HeaderSpeed.GHOST_NORM);
            }

            speed = percent * baseSpeed;
        }

        public override void draw(SpriteBatch sb)
        {
            if (pacman.CurrentMode != Pacman.Mode.DEAD)
            {
                if(currMode != Mode.DEAD)
                    sprite.draw(sb, pos, false, currDir == Direction.LEFT, false);

                if(currMode == Mode.DEAD)
                    sprEyes[Math.Min((int)currDir, 3)].draw(sb, new Vector2(pos.X + 10 - sprite.Center.X, pos.Y + 8 - sprite.Center.Y), false, false, false);
            }
        }

        public virtual int Compare(VecDir v1, VecDir v2)
        {
            Vector2 goal;

            if (currMode == Mode.CHASE)
                goal = new Vector2(Tile.toCenterCell(pacman.getPosition().X), Tile.toCenterCell(pacman.getPosition().Y));
            else if (currMode == Mode.LEAVE_PEN) 
                goal = new Vector2(Tile.toCenterCell(game.getMap().PenOut.X), Tile.toCenterCell(game.getMap().PenOut.Y));
            else if (currMode == Mode.DEAD)
                goal = new Vector2(Tile.toCenterCell(game.getMap().PenIn.X), Tile.toCenterCell(game.getMap().PenIn.Y));
            else
                goal = home;

            float d1 = (float) Math.Sqrt(Math.Pow(goal.X - v1.v.X, 2) + Math.Pow(goal.Y - v1.v.Y, 2));
            float d2 = (float) Math.Sqrt(Math.Pow(goal.X - v2.v.X, 2) + Math.Pow(goal.Y - v2.v.Y, 2));

            if (d1 < d2)
                return -1;
            if (d1 > d2)
                return 1;

            return 0;
        }

        public Vector2 Home
        {
            set { home = value; }
        }

        public Mode CurrentMode
        {
            get { return currMode; }
            set {
                    prevMode = currMode;
                    currMode = value;
            }
        }

        public Mode PreviousMode
        {
            get { return prevMode; }
        }
    }
}
