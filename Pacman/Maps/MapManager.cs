using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;

namespace PacmanGame.Maps
{
    class MapManager
    {
        Texture2D texAtlas;
        TileSet tileSet;

        // Contains all the maps
        Map[] maps;
        // Map parameters
        // Number of row in the map
        int numRow;
        // Number of column in the map
        int numCol;

        // Width in pixel of the map (numCol * Game.TILE_SIZE)
        int width;
        // Height in pixel of the map (numRow * Game.TILE_SIZE)
        int height;

        public MapManager(Texture2D texAtlas)
        {
            // Load tile map level
            XmlDocument docTileMap = new XmlDocument();
            docTileMap.Load(@"Content\pacman-levels.mpm");

            XmlDocument docMetaData = new XmlDocument();
            docMetaData.Load(@"Content\tileset_meta.xml");

            XmlElement elemMeta = (XmlElement)docMetaData.GetElementsByTagName("MetaData").Item(0);
            XmlElement elemTileSet = (XmlElement)docTileMap.GetElementsByTagName("TileSet").Item(0);

            int tsNumRow = int.Parse(elemTileSet.GetAttribute("num-row"));
            int tsNumCol = int.Parse(elemTileSet.GetAttribute("num-col"));
            int tsOffX = int.Parse(elemTileSet.GetAttribute("marginX"));
            int tsOffY = int.Parse(elemTileSet.GetAttribute("marginY"));
            int tsPadding = int.Parse(elemTileSet.GetAttribute("spacing"));
            int tileW = int.Parse(elemTileSet.GetAttribute("tile-width"));
            int tileH = int.Parse(elemTileSet.GetAttribute("tile-height"));

            tileSet = new TileSet(elemMeta, tileW, tileH, tsNumRow, tsNumCol, tsOffX, tsOffY, tsPadding, tsPadding);

            // Parse the MP tile map
            XmlElement elemtileMap = (XmlElement)docTileMap.GetElementsByTagName("TileMap").Item(0);

            // Get the dimensions of the map
            numRow = int.Parse(elemtileMap.GetAttribute("height"));
            numCol = int.Parse(elemtileMap.GetAttribute("width"));
            width = numCol * Game.TILE_SIZE;
            height = numRow * Game.TILE_SIZE;

            XmlNodeList listLayer = elemtileMap.GetElementsByTagName("Layer");
            XmlNodeList listObjectLayer = ((XmlElement)docTileMap.GetElementsByTagName("MapObjects").Item(0)).GetElementsByTagName("level");

            maps = new Map[listLayer.Count];

            // Parse layers (1 layer = 1 map)
            for (int layer = 0; layer < listLayer.Count; layer++)
            {
                XmlElement elemLayer = (XmlElement)listLayer.Item(layer);
                XmlElement elemObjects = (XmlElement)listObjectLayer.Item(layer);

                maps[layer] = new Map(elemLayer, elemObjects, texAtlas, tileSet, numRow, numCol);
            }
        }

        public Map[] Maps
        {
            get { return maps; }
        }

        public TileSet GetTileSet
        {
            get { return tileSet; }
        }
    }

    class TileSet
    {
        /// <summary>
        /// Simple struct holding meta data of the tiles
        /// </summary>
        public struct TileMeta
        {
            public string nature;
            public string type;

            public TileMeta(string nature, string type)
            {
                this.nature = nature;
                this.type = type;
            }
        }

        // Dictionary containing the meta data of each tile id
        Dictionary<int, TileMeta> metaData;

        Rectangle[] tileRegions;

        public TileSet(XmlElement elemTileMeta, int tileW, int tileH, int numRow, int numCol, int offX, int offY, int padX, int padY)
        {

            XmlNodeList listTile = elemTileMeta.GetElementsByTagName("Tile");

            metaData = new Dictionary<int, TileMeta>();
            metaData.Add(0, new TileMeta("normal", "normal"));

            for (int i = 0; i < listTile.Count; i++)
            {
                if (listTile.Item(i).NodeType == XmlNodeType.Element)
                {
                    XmlElement elemTile = (XmlElement)listTile.Item(i);

                    metaData.Add(int.Parse(elemTile.GetAttribute("id")), new TileMeta(elemTile.GetAttribute("nature"), elemTile.GetAttribute("type")));
                }
            }

            tileRegions = new Rectangle[numRow * numCol + 1];

            tileRegions[0] = new Rectangle();

            int index = 1;

            for (int row = 0; row < numRow; row++)
            {
                for (int col = 0; col < numCol; col++)
                {
                    tileRegions[index] = new Rectangle(offX + col * (tileW + padX), offY + row * (tileH + padY), tileW, tileH);
                    index++;
                }
            }
        }

        public Rectangle[] TileRegions
        {
            get { return tileRegions; }
        }

        public Dictionary<int, TileMeta> MetaData
        {
            get { return metaData; }
        }
    }
}
