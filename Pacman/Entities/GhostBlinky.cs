using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PacmanGame.GameStates;
using PacmanGame.Maps;
using PacmanGame.Utils;
using System;

namespace PacmanGame.Entities
{
    class GhostBlinky : Ghost
    {
        public GhostBlinky(Texture2D texAtlas, PlayState game, Pacman pacman) : base(texAtlas, game, pacman)
        {
            Rectangle[] moveFramesSide = new Rectangle[3];
            for (int i = 0; i < moveFramesSide.Length; i++)
            {
                moveFramesSide[i] = new Rectangle(1 + i * 37, 141, 36, 37);
            }

            Rectangle[] moveFramesUp= new Rectangle[3];
            for (int i = 0; i < moveFramesUp.Length; i++)
            {
                moveFramesUp[i] = new Rectangle(1 + i * 37, 217, 36, 37);
            }

            Rectangle[] moveFramesDown = new Rectangle[3];
            for (int i = 0; i < moveFramesDown.Length; i++)
            {
                moveFramesDown[i] = new Rectangle(1 + i * 37, 65, 36, 37);
            }

            Rectangle[] moveFramesSideChase = new Rectangle[3];
            for (int i = 0; i < moveFramesSideChase.Length; i++)
            {
                moveFramesSideChase[i] = new Rectangle(1 + i * 37, 179, 36, 37);
            }

            Rectangle[] moveFramesDownChase = new Rectangle[3];
            for (int i = 0; i < moveFramesDownChase.Length; i++)
            {
                moveFramesDownChase[i] = new Rectangle(1 + i * 37, 103, 36, 37);
            }

            anims[ANIM_MOVE_SIDE] = new Animation(sprite, moveFramesSide, 100, true);
            anims[ANIM_MOVE_UP] = new Animation(sprite, moveFramesUp, 100, true);
            anims[ANIM_MOVE_DOWN] = new Animation(sprite, moveFramesDown, 100, true);
            anims[ANIM_MOVE_SIDE_CHASE] = new Animation(sprite, moveFramesSideChase, 100, true);
            anims[ANIM_MOVE_DOWN_CHASE] = new Animation(sprite, moveFramesDownChase, 100, true);

            sprite.TextureRegion = moveFramesDown[0];

            currAnim = anims[ANIM_MOVE_DOWN];
            currAnimIndex = ANIM_MOVE_DOWN;
        }

        public override int Compare(VecDir v1, VecDir v2)
        {
            Vector2 goal;

            if (currMode == Mode.LEAVE_PEN)
                goal = new Vector2(Tile.toCenterCell(game.getMap().PenOut.X), Tile.toCenterCell(game.getMap().PenOut.Y));
            else if (currMode == Mode.DEAD)
                goal = new Vector2(Tile.toCenterCell(game.getMap().PenIn.X), Tile.toCenterCell(game.getMap().PenIn.Y));
            else if (currMode == Mode.CHASE || isElroyMode())
                goal = new Vector2(Tile.toCenterCell(pacman.getPosition().X), Tile.toCenterCell(pacman.getPosition().Y));
            else
                goal = home;

            float d1 = (float)Math.Sqrt(Math.Pow(goal.X - v1.v.X, 2) + Math.Pow(goal.Y - v1.v.Y, 2));
            float d2 = (float)Math.Sqrt(Math.Pow(goal.X - v2.v.X, 2) + Math.Pow(goal.Y - v2.v.Y, 2));

            if (d1 < d2)
                return -1;
            if (d1 > d2)
                return 1;

            return 0;
        }

        protected override void updateSpeed()
        {
            float percent = 1;

            if (currMode == Mode.FRIGHT)
            {
                percent = GameData.getSpeed(game.RuleManager.Round, GameData.HeaderSpeed.GHOST_FRIGHT);
            }
            else if (currMode == Mode.IN_PEN || currMode == Mode.LEAVE_PEN)
            {
                percent = GameData.getSpeed(game.RuleManager.Round, GameData.HeaderSpeed.GHOST_PEN);
            }
            else if (game.getMap().getTileNature(pos) == "tunnel")
            {
                percent = GameData.getSpeed(game.RuleManager.Round, GameData.HeaderSpeed.GHOST_TUNNEL);
            }
            else if (isElroyMode())
            {
                if(game.RuleManager.DotsLeftPercent <= GameData.getElroyPercent(game.RuleManager.Round, GameData.ElroyHeader.ELROY_2))
                    percent = GameData.getSpeed(game.RuleManager.Round, GameData.HeaderSpeed.ELROY_2);
                else if (game.RuleManager.DotsLeftPercent <= GameData.getElroyPercent(game.RuleManager.Round, GameData.ElroyHeader.ELROY_1))
                    percent = GameData.getSpeed(game.RuleManager.Round, GameData.HeaderSpeed.ELROY_1);
            }
            else
            {
                percent = GameData.getSpeed(game.RuleManager.Round, GameData.HeaderSpeed.GHOST_NORM);
            }

            speed = percent * baseSpeed;
        }

        private bool isElroyMode()
        {
            return game.RuleManager.DotsLeftPercent <= GameData.getElroyPercent(game.RuleManager.Round, GameData.ElroyHeader.ELROY_1) ||
                    game.RuleManager.DotsLeftPercent <= GameData.getElroyPercent(game.RuleManager.Round, GameData.ElroyHeader.ELROY_2);
        }
    }
}
