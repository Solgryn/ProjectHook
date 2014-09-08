using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;

namespace LevelReader.GameFrameWork
{
    public class TiledMap
    {

        #region Variables
        private Dictionary<String, int[,]> _layers;
        private List<String> _layerNames;
        private int _width;
        private int _height;
        private int _tileWidth;
        private int _tileHeight;
        private string _orientation;
        private Color _backgroundcolor;
        private int _tileSetSpacing;
        private int _imageWidth;

        #endregion

        #region Properties
        public int NumberOfLevels { get {return _layers.Count;} }

        public Dictionary<string, int[,]> Layers { get { return _layers; } }

        public int Width { get { return _width; } }

        public int Height { get { return _height; } } 

        public String[] LayerNames { get { return _layerNames.ToArray(); } }

        public int TileWidth { get { return _tileWidth; } }

        public int TileHeight { get { return _tileHeight; } }

        public string Orientation
        {
            get { return _orientation; }
        }

        public Color BackgroundColor
        {
            get { return _backgroundcolor; }
        }

        public int TileSetSpacing
        {
            get { return _tileSetSpacing; }
        }

        public int ImageWidth
        {
            get { return _imageWidth; }
        }

        #endregion

        #region Constructor
        public TiledMap(String xmlFile)
        {
            try
            {
                XmlReader reader = XmlReader.Create(xmlFile);

                _layerNames = new List<string>();
                _layers = new Dictionary<string, int[,]>();

                Queue<int> tempTileValue = new Queue<int>();

                while (reader.Read())
                    if (reader.IsStartElement())
                    {
                        if (reader.Name == "map")
                        {
                            _orientation = (reader.GetAttribute("orientation"));
                            _width = Convert.ToInt32(reader.GetAttribute("width"));
                            _height = Convert.ToInt32(reader.GetAttribute("height"));
                            _tileWidth = Convert.ToInt32(reader.GetAttribute("tilewidth"));
                            _tileHeight = Convert.ToInt32(reader.GetAttribute("tileheight"));
                            string backgroundColor = reader.GetAttribute("backgroundcolor");
                            if (backgroundColor != null)
                            _backgroundcolor = new Color(
                                Convert.ToInt32(backgroundColor.Substring(1, 2), 16),
                                Convert.ToInt32(backgroundColor.Substring(3, 2), 16),
                                Convert.ToInt32(backgroundColor.Substring(5, 2), 16));
                            }

                        if (reader.Name == "tileset")
                        {
                            _tileSetSpacing = Convert.ToInt32(reader.GetAttribute("spacing"));
                        }

                        if (reader.Name == "image")
                        {
                            _imageWidth = Convert.ToInt32(reader.GetAttribute("width"));
                        }

                        if (reader.Name == "layer")
                        {
                            string name = reader.GetAttribute("name");
                            _layerNames.Add(name);
                            _layers.Add(name, new int[Height, Width]);
                        }

                        if (reader.Name == "tile")
                        {
                            // add tile value to the temporary list
                            tempTileValue.Enqueue(Convert.ToInt32(reader.GetAttribute("gid")));
                        }

                        // if temp list holds full layer
                        if (tempTileValue.Count == _width * _height)
                        {
                            // add layer tiles from temporary list to layer
                            for (int i = 0; i < _height; i++)
                            {
                                for (int j = 0; j < _width; j++)
                                {
                                    int[,] layer = _layers[_layerNames[_layerNames.Count-1]];
                                    layer[i, j] = tempTileValue.Dequeue();
                                }
                            }
                        }
                    }
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("Path to tmx level is wrong, have you remembered to include the level in the project properly?");
            }
            catch (Exception )
            {
                throw;
                //throw new FormatException("Tmx level has wrong format - did you use xml as output in the editor?");
            }
        }
        #endregion


    }
}
