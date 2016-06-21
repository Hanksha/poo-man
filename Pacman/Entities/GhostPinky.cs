﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PacmanGame.GameStates;
using PacmanGame.Maps;
using PacmanGame.Utils;
using System;

namespace PacmanGame.Entities
{
    class GhostPinky : Ghost
    {
        public GhostPinky(Texture2D texAtlas, PlayState game, Pacman pacman) : base(texAtlas, game, pacman)
        {
            Rectangle[] moveFramesSide = new Rectangle[3];
            for (int i = 0; i < moveFramesSide.Length; i++)
            {
                moveFramesSide[i] = new Rectangle(1 + i * 37, 331, 36, 37);
            }

            Rectangle[] moveFramesUp = new Rectangle[3];
            for (int i = 0; i < moveFramesUp.Length; i++)
            {
                moveFramesUp[i] = new Rectangle(1 + i * 37, 407, 36, 37);
            }

            Rectangle[] moveFramesDown = new Rectangle[3];
            for (int i = 0; i < moveFramesDown.Length; i++)
            {
                moveFramesDown[i] = new Rectangle(1 + i * 37, 255, 36, 37);
            }

            Rectangle[] moveFramesSideChase = new Rectangle[3];
            for (int i = 0; i < moveFramesSideChase.Length; i++)
            {
                moveFramesSideChase[i] = new Rectangle(1 + i * 37, 369, 36, 37);
            }

            Rectangle[] moveFramesDownChase = new Rectangle[3];
            for (int i = 0; i < moveFramesDown.Length; i++)
            {
                moveFramesDownChase[i] = new Rectangle(1 + i * 37, 293, 36, 37);
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
            else if (currMode == Mode.CHASE)
            {
                if(game.getPacman().CurrDirection == Direction.LEFT)
                    goal = new Vector2(Tile.toCenterCell(pacman.getPosition().X) - Game.TILE_SIZE * 4, Tile.toCenterCell(pacman.getPosition().Y));
                else if(game.getPacman().CurrDirection == Direction.RIGHT)
                    goal = new Vector2(Tile.toCenterCell(pacman.getPosition().X) + Game.TILE_SIZE * 4, Tile.toCenterCell(pacman.getPosition().Y));
                else if (game.getPacman().CurrDirection == Direction.UP)
                    goal = new Vector2(Tile.toCenterCell(pacman.getPosition().X), Tile.toCenterCell(pacman.getPosition().Y) - Game.TILE_SIZE * 4);
                else
                    goal = new Vector2(Tile.toCenterCell(pacman.getPosition().X), Tile.toCenterCell(pacman.getPosition().Y) + Game.TILE_SIZE * 4);
            }
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
    }
}
