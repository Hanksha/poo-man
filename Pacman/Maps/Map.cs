using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PacmanGame.GameStates;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using static PacmanGame.Game;

namespace PacmanGame.Maps
{
    class Map
    {
        // Reference to the texture atlas
        Texture2D texAtlas;
        // Reference to the tile set
        TileSet tileSet;

        XmlElement elemLayer;

        Tile[,] grid;

        string name;

        // Start positions
        Vector2 playerStart;
        Vector2 blinkyStart;
        Vector2 blinkyHome;
        Vector2 pinkyStart;
        Vector2 pinkyHome;
        Vector2 inkyStart;
        Vector2 inkyHome;
        Vector2 clydeHome;
        Vector2 clydeStart;
        Vector2 penIn;
        Vector2 penOut;
        Vector2 bonusPos;

        // Dot and energizer count
        int dotCount;

        public Map(XmlElement elemLayer, XmlElement elemObjects, Texture2D texAtlas, TileSet tileSet, int numRow, int numCol)
        {
            this.elemLayer = elemLayer;
            this.texAtlas = texAtlas;
            this.tileSet = tileSet;

            //Get start positions, homes, and pen in/out
            int row, col;
            XmlElement elem;

            name = elemLayer.GetAttribute("name");

            elem = (XmlElement) elemObjects.GetElementsByTagName("pacman_start").Item(0);
            row = int.Parse(elem.GetAttribute("row"));
            col = int.Parse(elem.GetAttribute("col"));
            playerStart = new Vector2(col * TILE_SIZE + TILE_SIZE / 2, row * TILE_SIZE + TILE_SIZE / 2);

            elem = (XmlElement)elemObjects.GetElementsByTagName("blinky_start").Item(0);
            row = int.Parse(elem.GetAttribute("row"));
            col = int.Parse(elem.GetAttribute("col"));
            blinkyStart = new Vector2(col * TILE_SIZE + TILE_SIZE / 2, row * TILE_SIZE + TILE_SIZE / 2);
            elem = (XmlElement)elemObjects.GetElementsByTagName("blinky_home").Item(0);
            row = int.Parse(elem.GetAttribute("row"));
            col = int.Parse(elem.GetAttribute("col"));
            blinkyHome = new Vector2(col * TILE_SIZE + TILE_SIZE / 2, row * TILE_SIZE + TILE_SIZE / 2);

            elem = (XmlElement)elemObjects.GetElementsByTagName("pinky_start").Item(0);
            row = int.Parse(elem.GetAttribute("row"));
            col = int.Parse(elem.GetAttribute("col"));
            pinkyStart = new Vector2(col * TILE_SIZE + TILE_SIZE / 2, row * TILE_SIZE + TILE_SIZE / 2);
            elem = (XmlElement)elemObjects.GetElementsByTagName("pinky_home").Item(0);
            row = int.Parse(elem.GetAttribute("row"));
            col = int.Parse(elem.GetAttribute("col"));
            pinkyHome = new Vector2(col * TILE_SIZE + TILE_SIZE / 2, row * TILE_SIZE + TILE_SIZE / 2);

            elem = (XmlElement)elemObjects.GetElementsByTagName("inky_start").Item(0);
            row = int.Parse(elem.GetAttribute("row"));
            col = int.Parse(elem.GetAttribute("col"));
            inkyStart = new Vector2(col * TILE_SIZE + TILE_SIZE / 2, row * TILE_SIZE + TILE_SIZE / 2);
            elem = (XmlElement)elemObjects.GetElementsByTagName("inky_home").Item(0);
            row = int.Parse(elem.GetAttribute("row"));
            col = int.Parse(elem.GetAttribute("col"));
            inkyHome = new Vector2(col * TILE_SIZE + TILE_SIZE / 2, row * TILE_SIZE + TILE_SIZE / 2);

            elem = (XmlElement)elemObjects.GetElementsByTagName("clyde_start").Item(0);
            row = int.Parse(elem.GetAttribute("row"));
            col = int.Parse(elem.GetAttribute("col"));
            clydeStart = new Vector2(col * TILE_SIZE + TILE_SIZE / 2, row * TILE_SIZE + TILE_SIZE / 2);
            elem = (XmlElement)elemObjects.GetElementsByTagName("clyde_home").Item(0);
            row = int.Parse(elem.GetAttribute("row"));
            col = int.Parse(elem.GetAttribute("col"));
            clydeHome = new Vector2(col * TILE_SIZE + TILE_SIZE / 2, row * TILE_SIZE + TILE_SIZE / 2);


            elem = (XmlElement)elemObjects.GetElementsByTagName("pen_in").Item(0);
            row = int.Parse(elem.GetAttribute("row"));
            col = int.Parse(elem.GetAttribute("col"));
            penIn = new Vector2(col * TILE_SIZE + TILE_SIZE / 2, row * TILE_SIZE + TILE_SIZE / 2);
            elem = (XmlElement)elemObjects.GetElementsByTagName("pen_out").Item(0);
            row = int.Parse(elem.GetAttribute("row"));
            col = int.Parse(elem.GetAttribute("col"));
            penOut = new Vector2(col * TILE_SIZE + TILE_SIZE / 2, row * TILE_SIZE + TILE_SIZE / 2);

            elem = (XmlElement)elemObjects.GetElementsByTagName("bonus_pos").Item(0);
            row = int.Parse(elem.GetAttribute("row"));
            col = int.Parse(elem.GetAttribute("col"));
            bonusPos = new Vector2(col * TILE_SIZE + TILE_SIZE / 2, row * TILE_SIZE + TILE_SIZE / 2);

            grid = new Tile[numRow, numCol];

            load();
        }

        public void load()
        {
            dotCount = 0;

            XmlNodeList listRows = elemLayer.GetElementsByTagName("Row");

            for (int row = 0; row < grid.GetLength(0); row++)
            {
                string[] tokens = listRows.Item(row).InnerText.Split(',');
                for (int col = 0; col < grid.GetLength(1); col++)
                {
                    int id = int.Parse(tokens[col]);
                    int rawId = Tile.getRawId(id);

                    if (tileSet.MetaData[rawId].nature == "dot" || tileSet.MetaData[rawId].nature == "energizer")
                    {
                        dotCount++;
                    }

                    grid[row, col] = new Tile(rawId,
                                              Tile.isTrans((uint)id, Tile.ROTATE),
                                              Tile.isTrans((uint)id, Tile.FLIPH),
                                              Tile.isTrans((uint)id, Tile.FLIPV)
                                              );
                }
            }
        }

        public void draw(SpriteBatch sb)
        {

            Rectangle pos = new Rectangle(0, 0, 32, 32);
            Tile tile = null;

            for (int row = 0; row < grid.GetLength(0); row++)
            {
                for (int col = 0; col < grid.GetLength(1); col++)
                {
                    tile = grid[row, col];


                    pos.X = col * Game.TILE_SIZE + 16;
                    pos.Y = row * Game.TILE_SIZE + 16;

                    sb.Draw(texAtlas,
                        pos,
                        tileSet.TileRegions[19],
                        Color.White,
                        0,
                        new Vector2(16, 16),
                        SpriteEffects.None,
                        0
                        );

                    if (tile.id == 0)
                        continue;

                    SpriteEffects effect = SpriteEffects.None;

                    if (tile.flipX)
                        effect |= SpriteEffects.FlipHorizontally;

                    if (tile.flipY)
                        effect |= SpriteEffects.FlipVertically;

                    float rotAngle = 0;

                    if (tile.rotate)
                    {
                        rotAngle = (float)(Math.PI / 2);

                        if (tile.flipX && tile.flipY)
                        {
                        }
                        else if (tile.flipX || tile.flipY)
                        {
                            rotAngle += (float)(Math.PI);
                        }
                    }

                    sb.Draw(texAtlas,
                        pos,
                        tileSet.TileRegions[tile.id],
                        Color.White,
                        rotAngle,
                        new Vector2(16, 16),
                        effect,
                        0
                        );
                }
            }

        }

        public void setTile(int id, int row, int col)
        {
            grid[row, col].id = id;
        }

        public string getTileNature(Vector2 pos)
        {
            int row = Tile.toCell(pos.Y);
            int col = Tile.toCell(pos.X);

            if (row < 0 || row >= NumRow)
                return "normal";
            if (col < 0 || col >= NumCol)
                return "normal";

            return tileSet.MetaData[grid[row, col].id].nature;
        }

        public string Name
        {
            get { return name; }
        }

        public Vector2 PacmanStart
        {
            get { return playerStart; }
        }

        public Vector2 BlinkyStart
        {
            get { return blinkyStart; }
        }

        public Vector2 BlinkyHome
        {
            get { return blinkyHome; }
        }

        public Vector2 PinkyStart
        {
            get { return pinkyStart; }
        }

        public Vector2 PinkyHome
        {
            get { return pinkyHome; }
        }

        public Vector2 InkyStart
        {
            get { return inkyStart; }
        }

        public Vector2 InkyHome
        {
            get { return inkyHome; }
        }

        public Vector2 ClydeStart
        {
            get { return clydeStart; }
        }

        public Vector2 ClydeHome
        {
            get { return clydeHome; }
        }

        public Vector2 PenIn
        {
            get { return penIn; }
        }

        public Vector2 PenOut
        {
            get { return penOut; }
        }

        public Vector2 BonusPosition
        {
            get { return bonusPos; }
        }

        public Tile[,] Grid
        {
            get { return grid; }
        }

        public TileSet Tiles
        {
            get { return tileSet; }
        }

        public int DotCount
        {
            get {return dotCount;}
        }

        public int Width
        {
            get { return grid.GetLength(1) * Game.TILE_SIZE; }
        }

        public int Height
        {
            get { return grid.GetLength(0) * Game.TILE_SIZE; }
        }
        
        public int NumRow
        {
            get { return grid.GetLength(0); }
        }

        public int NumCol
        {
            get { return grid.GetLength(1); }
        }
    }
}
