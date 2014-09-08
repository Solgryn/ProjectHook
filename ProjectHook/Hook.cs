using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectHook.GameFrameWork;

namespace ProjectHook
{
    public sealed class Hook : SpriteObject
    {
        public Vector2 Pull = new Vector2(10, -7.5f);
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

            //Set origin
            if (spriteEffects == SpriteEffects.FlipHorizontally)
                OriginX = SpriteTexture.Width;

            OriginY = SpriteTexture.Height / 2.0f;

            //Set hitbox
            BoundingBox = new Rectangle(0, 24, SpriteTexture.Width, 16);
        }

        public override void Update(GameTime gameTime)
        {
            Position = _player.Position; //Set the hook to the player

            ScaleX = ScaleX + (1 - ScaleX)*0.25f; //Gradually scale hook outwards

            //Destroy hook after some time
            _life++;
            if (_life >= _maxLife)
                Destroy();


            //Hook hits another player
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

            //Hook hits solid
            if (OverlapsSolid(BoundingBox))
            {
                _player.HookHit(this);
                Destroy();
            }
        }
    }
}
