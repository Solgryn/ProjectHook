using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectHook.GameFrameWork;

namespace ProjectHook
{
    sealed class Cloud : SpriteObject
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
