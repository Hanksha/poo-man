using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PacmanGame.Utils;
using PacmanGame.GameStates;
using PacmanGame.Maps;
using System.Diagnostics;

namespace PacmanGame.Entities
{
    /// <summary>
    /// Reprents a moving entity.
    /// </summary>
    abstract class MovingEntity : Entity
    {
        protected PlayState game;
        protected Sprite sprite;

        /// <summary>
        /// Position on the map.
        /// </summary>
        protected Vector2 pos;
        protected float baseSpeed = 180;
        protected float speed = 180;
        protected Path path;
        protected Point currCell;
        public enum Direction {LEFT, RIGHT, UP, DOWN, NONE};
        protected Direction currDir = Direction.NONE;
        protected Direction reqDir = Direction.NONE;
        protected bool moving;

        public MovingEntity(PlayState game)
        {
            this.game = game;

            pos = new Vector2();

            path = new Path();
            path.set((int) pos.X, (int) pos.Y, (int) pos.X, (int) pos.Y);
            
            currCell = new Point();
        }
      
        protected void updateMovement(double dt)
        {

            if((reqDir == Direction.LEFT || reqDir == Direction.RIGHT) && 
               (currDir == Direction.UP || currDir == Direction.DOWN))
            {
                if(path.isDone())
                {
                    if (tryTurn(reqDir))
                        reqDir = Direction.NONE;
                }
            }
            else if ((reqDir == Direction.UP || reqDir == Direction.DOWN) &&
                     (currDir == Direction.LEFT || currDir == Direction.RIGHT))
            {
                if (path.isDone())
                {
                    if (tryTurn(reqDir))
                        reqDir = Direction.NONE;
                }
            }
            else
            {
                if (tryTurn(reqDir))
                    reqDir = Direction.NONE;
            }

            if (path.isDone())
            {
                endPath();

                if (path.isDone())
                    moving = false;
            }
            else
                moving = true;

            path.update((float) (speed * dt));

            pos = path.setOnPath(pos);

            if(currCell.X != Tile.toCell(pos.X) || currCell.Y != Tile.toCell(pos.Y))
            {
                currCell.X = Tile.toCell(pos.X);
                currCell.Y = Tile.toCell(pos.Y);

                enteredNewCell();
            }


            if(currDir == Direction.LEFT)
            {
                if (pos.X < 0)
                {
                    pos.X =  game.getMap().Width;

                    tryTurn(currDir);
                }
            }
            else if(currDir == Direction.RIGHT)
            {
                if(pos.X >= game.getMap().Width)
                {
                    pos.X = 0;

                    tryTurn(currDir);
                }
            }
            else if(currDir == Direction.UP)
            {
                if (pos.Y < 0)
                {
                    pos.Y = game.getMap().Height;

                    tryTurn(currDir);
                }
            }
            else if (currDir == Direction.DOWN)
            {
                if (pos.Y >= game.getMap().Height)
                {
                    pos.Y = 0;

                    tryTurn(currDir);
                }
            }
        }

        protected bool tryTurn(Direction dir)
        {
            if(dir == Direction.LEFT)
            {
                if (!isBlock(pos.X - 17, pos.Y))
                {
                    path.set(pos.X, pos.Y, Tile.toCenterCell(pos.X - 17), Tile.toCenterCell(pos.Y));

                    currDir = dir;
                    return true;
                }

            }
            else if(dir == Direction.RIGHT)
            {
                if (!isBlock(pos.X + 17, pos.Y))
                {
                    path.set(pos.X, pos.Y, Tile.toCenterCell(pos.X + 17), Tile.toCenterCell(pos.Y));

                    currDir = dir;
                    return true;
                }
            }
            else if (dir == Direction.UP)
            {
                if (!isBlock(pos.X, pos.Y - 17))
                {
                    path.set(pos.X, pos.Y, Tile.toCenterCell(pos.X), Tile.toCenterCell(pos.Y - 17));

                    currDir = dir;
                    return true;
                }
            }
            else if (dir == Direction.DOWN)
            {
                if (!isBlock(pos.X, pos.Y + 17))
                {
                    path.set(pos.X, pos.Y, Tile.toCenterCell(pos.X), Tile.toCenterCell(pos.Y + 17));

                    currDir = dir;
                    return true;
                }
            }

            return false;
        }

        protected virtual void enteredNewCell()
        {
            updateSpeed();
        }

        protected virtual void endPath()
        {

        }

        protected virtual void updateSpeed()
        {
            speed = baseSpeed;
        }

        /// <summary>
        /// Updates the entity, call base.update(double dt) if overriding.
        /// </summary>
        /// <param name="dt">Delta Time</param>
        public virtual void update(double dt)
        {
            updateMovement(dt);
        }

        public virtual void draw(SpriteBatch sb)
        {
            sprite.draw(sb, pos, currDir == Direction.DOWN || currDir == Direction.UP, currDir == Direction.LEFT, currDir == Direction.UP);
        }

        protected virtual bool isBlock(float x, float y)
        {
            int row = Tile.toCell(y);
            int col = Tile.toCell(x);

            if (row < 0 || row >= game.getMap().NumRow)
                return false;
            if (col < 0 || col >= game.getMap().NumCol)
                return false;

            return game.getMap().Tiles.MetaData[game.getMap().Grid[row, col].id].type == "block";
        }

        public void setPosition(float x, float y)
        {
            pos.X = x;
            pos.Y = y;

            path.set(x, y, x, y);

            currDir = Direction.NONE;
            reqDir = Direction.NONE;
        }


        public Vector2 getPosition()
        {
            return pos;
        }

        public float Speed
        {
            get { return speed; }
        }

        public Point CurrentCell
        {
            get { return currCell; }
        }

        public Direction CurrDirection
        {
            get { return currDir; }
        }
    }
}
