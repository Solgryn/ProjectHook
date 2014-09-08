using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectHook
{
    public class Tile : SpriteObject
    {
        public string LayerName;
        public Tile(GameHost game, Vector2 position, Texture2D texture, Rectangle sourceRect, string layerName)
            : base(game, position, texture)
        {
            SpriteTexture = texture;
            SourceRect = sourceRect;
            LayerName = layerName;
        }
    }
}
