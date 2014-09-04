using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectHook
{
    class Cloud : SpriteObject
    {

      public Cloud(GameHost game, Vector2 position, Texture2D texture)
            : base(game, position, texture)
        {
            SpriteTexture = texture;
            
        }

        public override void Update(GameTime gameTime)
        {
            PositionX++;
        }
    }

}
