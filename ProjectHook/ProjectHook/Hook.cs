using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectHook
{
    public class Hook : SpriteObject
    {
        public Vector2 Pull = new Vector2(10, -5);
        private readonly Player _player;
        private float _life;
        private readonly float _maxLife;

        public Hook(GameHost game, Vector2 position, Texture2D texture, Player player, float life)
            : base(game, position, texture)
        {
            SpriteTexture = texture;
            _player = player;
            _maxLife = life;

            spriteEffects = _player.spriteEffects;
            ScaleX = 0;

            if (spriteEffects == SpriteEffects.FlipHorizontally)
                OriginX = SpriteTexture.Width;

            OriginY = SpriteTexture.Height / 2.0f;
        }

        public override void Update(GameTime gameTime)
        {
            Position = _player.Position;

            //ScaleX = _life / _maxLife;
            ScaleX = ScaleX + (1 - ScaleX)*0.25f;
            _life++;
            if (_life == _maxLife)
                Destroy();

            foreach (var player in Collections.Players)
            {
                if (player != _player)
                {
                    if (Overlaps(player))
                    {
                        player.GetHit(this, _player);
                        Destroy();
                    }
                }

            }
        }
    }
}
