using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectHook.GameFrameWork;

namespace ProjectHook
{
    public sealed class Hook : SpriteObject
    {
        public Vector2 Pull = new Vector2(10, -3.5f);
        private readonly Player _player;
        private int _life;
        private readonly int _maxLife;
        private bool _canHit = true;

        public Hook(GameHost game, Vector2 position, Texture2D texture, Player player, int life)
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

            _life++;

            if(_life < _maxLife)
                ScaleX = ScaleX.SmoothTowards(1, 0.25f); //Gradually scale hook outwards

            if (_life >= _maxLife) //Hook goes back in after stretching out
            {
                _canHit = false;
                ScaleX = ScaleX.SmoothTowards(0, 0.25f);
            }

            if (_life >= _maxLife * 2) //Destroy hook after some time
                Destroy();

            //Hook hits another player
            foreach (var player in Collections.Players)
            {
                if (player != _player)
                {
                    if (Overlaps(player) && _canHit)
                    {
                        player.GetHit(this, _player);
                        _life = _maxLife;
                    }
                }
            }

            //Hook hits solid
            if (OverlapsTileLayer(BoundingBox, Globals.LAYER_SOLID) && _canHit)
            {
                _player.HookHit(this);
                _life = _maxLife;
            }
        }
    }
}
