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

            foreach (var layername in _tiledMap.LayerNames) //For each layer
            {
                for (var i = 0; i < _tiledMap.Width; i++) //For each tile on the X axis
                {
                    for (var j = 0; j < _tiledMap.Height; j++) //For each tile on the Y axis
                    {
                        var gidValue = _tiledMap.Layers[layername][j, i]; //Get the gid value

                        if (gidValue != 0) //If gid value is 0, there's no tile, continue the for loops
                        {
                            var currentTileType = ""; //Default type is nothing

                            //If the current tile is in the dictionary for tiles containing types, grab the type
                            if (tileTypes.ContainsKey(gidValue))
                                currentTileType = tileTypes[gidValue];

                            _tilesPrRow = Math.Max(_tilesPrRow, 1);
                            var x = (gidValue - 1) % _tilesPrRow * (_tiledMap.TileWidth + _tiledMap.TileSetSpacing); //Get x position
                            var y = ((gidValue - 1) / _tilesPrRow) * (_tiledMap.TileHeight + _tiledMap.TileSetSpacing); //Get y position
                            var tile = new Tile
                                (Game,
                                new Vector2(i * _tiledMap.TileWidth * ScaleX + PositionX, j * _tiledMap.TileHeight * ScaleY + PositionY),
                                SpriteTexture,
                                new Rectangle(x, y, _tiledMap.TileWidth, _tiledMap.TileHeight),
                                layername,
                                currentTileType);
                            
                            //Add it to the collection for drawing
                            Collections.Tiles.Add(tile);
                        }
                    }
                }
            }
        }
    }
}
