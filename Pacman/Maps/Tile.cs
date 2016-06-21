using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PacmanGame.Maps
{
    class Tile
    {
        public const uint BITMASK_FLIPH = 0x80000000;
        public const uint BITMASK_FLIPV = 0x40000000;
        public const uint BITMASK_ROTATE = 0x20000000;
        public const int FLIPH = 0;
        public const int FLIPV = 1;
        public const int ROTATE = 2;

        public int id;

        public bool rotate, flipX, flipY;

        public Tile(int id, bool rotate, bool flipX, bool flipY)
        {
            this.id = id;
            this.rotate = rotate;
            this.flipX = flipX;
            this.flipY = flipY;
        }

        public static bool isTrans(uint id, int param)
        {
            if (param == FLIPH)
            {
                return (id & BITMASK_FLIPH) == BITMASK_FLIPH;
            }
            else if (param == FLIPV)
            {
                return (id & BITMASK_FLIPV) == BITMASK_FLIPV;
            }
            else if (param == ROTATE)
            {
                return (id & BITMASK_ROTATE) == BITMASK_ROTATE;
            }
            else
                return false;
        }

        // return the tile raw id
        public static int getRawId(int id)
        {
            id <<= 3;
            id = (int)((uint)id) >> 3;

            return id;
        }

        public static int toCell(float value)
        {
            return (int)(value / Game.TILE_SIZE);
        }

        public static int toCenterCell(float value)
        {
            return (int)(value / Game.TILE_SIZE) * Game.TILE_SIZE + (value > 0 ? Game.TILE_SIZE : -Game.TILE_SIZE) / 2;
        }
    }
}
