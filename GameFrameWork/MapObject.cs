using System;
using System.Diagnostics;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectHook;
using ProjectHook.GameFrameWork;

namespace LevelReader.GameFrameWork
{
 /// <summary>
 /// This class is used to draw tiled based maps contained in a TiledMap class
 /// supported Position,  scale, draw
 /// </summary>
    class MapObject : SpriteObject
    {
        private TiledMap _tiledMap;
        private int _tilesPrRow;

        public MapObject(GameHost game, Vector2 position, Texture2D texture, TiledMap tiledMap)
            : base(game, position, texture)
        {

            _tiledMap = tiledMap;
            _tilesPrRow = _tiledMap.ImageWidth / (_tiledMap.TileWidth + _tiledMap.TileSetSpacing);
            var tileTypes = _tiledMap.TileTypes;

            foreach (var layername in _tiledMap.LayerNames)
            {
                for (var i = 0; i < _tiledMap.Width; i++)
                {
                    for (var j = 0; j < _tiledMap.Height; j++)
                    {
                        var gidValue = _tiledMap.Layers[layername][j, i];

                        if (gidValue != 0)
                        {
                            var currentTileType = "";
                            if (tileTypes.ContainsKey(gidValue))
                                currentTileType = tileTypes[gidValue];

                            _tilesPrRow = Math.Max(_tilesPrRow, 1);
                            var x = (gidValue - 1) % _tilesPrRow * (_tiledMap.TileWidth + _tiledMap.TileSetSpacing);
                            var y = ((gidValue - 1) / _tilesPrRow) * (_tiledMap.TileHeight + _tiledMap.TileSetSpacing);
                            var tile = new Tile
                                (Game,
                                new Vector2(i * _tiledMap.TileWidth * ScaleX + PositionX, j * _tiledMap.TileHeight * ScaleY + PositionY),
                                SpriteTexture,
                                new Rectangle(x, y, _tiledMap.TileWidth, _tiledMap.TileHeight),
                                layername,
                                currentTileType);
                            

                            Collections.Tiles.Add(tile);
                        }
                    }
                }
            }
        }
    }
}
