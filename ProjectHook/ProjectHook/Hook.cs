using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectHook
{
    class Hook : SpriteObject
    {
        public Vector2 Velocity;
        public Hook(GameHost game, Vector2 position, Texture2D texture, Vector2 velocity)
            : base(game, position, texture)
        {
            SpriteTexture = texture;
            Velocity = velocity;

        }

        public override void Update(GameTime gameTime)
        {
            Position += Velocity;
        }
    }
}
