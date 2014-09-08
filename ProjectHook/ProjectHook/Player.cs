using System;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectHook
{
    sealed class Player : SpriteObject
    {
        //Hej :D
        public PlayerIndex PlayerIndex;

        public Vector2 Velocity = new Vector2(0, 0);
        public float Acceleration = 0.6f;
        public float Deacceleration = 0.4f;
        public float MaxSpeed = 3.5f;
        public float JumpStrength = -10f;
        public bool OnGround = false;

        private double _animationCounter;
        private double _animationSpeed;
        private int _currentFrame;

        private bool _jumpKey;
        private bool _grappleKey;
        private Cooldown grappleCooldown = new Cooldown(25);

        //Animations
        public Animation CurrentAnimation = Animations.Idle;

        internal class Animations
        {
            public static readonly Animation Idle = new Animation(15, new [] { 0 });
            public static readonly Animation Running = new Animation(15, new[] { 0, 1, 0, 2 });
            public static readonly Animation Jumping = new Animation(15, new[] { 3 });
        }

        public Player(GameHost game, Vector2 position, Texture2D texture, PlayerIndex playerIndex)
            : base(game, position, texture)
        {
            SpriteTexture = texture;
            PlayerIndex = playerIndex;

        }

        public override void Update(GameTime gameTime)
        {
            #region Cooldowns
            grappleCooldown.Decrement();
            #endregion

            #region Controls
            //Use either gamepad or keyboard
            float controlX;
            if (GamePad.GetState(PlayerIndex).IsConnected)
            {
                controlX = GamePad.GetState(PlayerIndex).ThumbSticks.Left.X;
                _jumpKey = GamePad.GetState(PlayerIndex).IsButtonDown(Buttons.A);
                _grappleKey = GamePad.GetState(PlayerIndex).IsButtonDown(Buttons.B);
            }
            else
            {
                controlX = 0;
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    controlX = -1;
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    controlX = 1;
                _jumpKey = Keyboard.GetState().IsKeyDown(Keys.Z);
                _grappleKey = Keyboard.GetState().IsKeyDown(Keys.X);
            }

            //Controls
            
            //Left
            if (controlX < 0)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
                Velocity.X = Math.Max(Velocity.X - Acceleration, controlX * MaxSpeed);
            }
            else if (Velocity.X < 0)
            {
                Velocity.X = Math.Min(Velocity.X + Deacceleration, 0);
            }

            //Right
            if (controlX > 0)
            {
                spriteEffects = SpriteEffects.None;
                Velocity.X = Math.Min(Velocity.X + Acceleration, controlX * MaxSpeed);
            }
            else if (Velocity.X > 0)
            {
                Velocity.X = Math.Max(Velocity.X - Deacceleration, 0);
            }

            //Jump
            if (_jumpKey && OnGround)
            {

                Velocity.Y = JumpStrength;
                OnGround = false;
                
            }
            else if(Velocity.Y < 0 && !_jumpKey)
            {
                Velocity.Y *=0.85f;
            }
            
            //Hook
            if (_grappleKey && grappleCooldown.IsOff())
            {
                ThrowHook();
                grappleCooldown.GoOnCooldown();
            }
            #endregion

            #region Moving the player
            //Apply Gravity
            if(!OnGround)
                Velocity.Y += 0.35f;

            //X
            var temp = Math.Abs(Velocity.X);
            var way = MathHelper.Clamp(Velocity.X, -1, 1);
            for (var i = 0; i < Math.Abs(Velocity.X); i++)
            {
                if (temp < 1)
                {
                    PositionX += temp*way;
                    break;
                }
                PositionX+=way;
                temp--;
            }

            //Y
            temp = Math.Abs(Velocity.Y);
            way = MathHelper.Clamp(Velocity.Y, -1, 1);
            for (var i = 0; i < Math.Abs(Velocity.Y); i++)
            {
                if (PositionY > 350 && Velocity.Y > 0)
                {
                    OnGround = true;
                    Velocity.Y = 0;
                    break;
                }
                if (temp < 1)
                {
                    PositionY+=temp*way;
                    break;
                }
                PositionY+=way;
                temp--;
            }
            #endregion

            #region Animations
            //Set animations
            if (Velocity.X == 0 && OnGround)
                ChangeAnimation(Animations.Idle);
            if ((Velocity.X > 0 || Velocity.X < 0) && OnGround)
                ChangeAnimation(Animations.Running);
            if (!OnGround)
                ChangeAnimation((Animations.Jumping));

                
            
            //Animation frame
            _animationCounter += _animationSpeed;
            if (_animationCounter >= 100)
            {
                _animationCounter = 0;
                _currentFrame++;
                if (_currentFrame == CurrentAnimation.Length)
                    _currentFrame = 0;
            }
            #endregion

            //Set texture image
            SourceRect = new Rectangle(0, (CurrentAnimation.Frames[_currentFrame]*64)+1, 64, 64);
        }

        public void ChangeAnimation(Animation animation)
        {
            if (CurrentAnimation.IsEqual(animation)) return;
            CurrentAnimation = animation;
            _animationSpeed = animation.Speed;
            _currentFrame = 0;
        }

        public void ThrowHook()
        {
            var hook = new Hook(Game, Position, Game.Content.Load<Texture2D>("grapple"), new Vector2(GetDirection()*10, 0));
            Game.GameObjects.Add(hook);
        }

        public int GetDirection()
        {
            if (spriteEffects == SpriteEffects.FlipHorizontally) return -1;
            if (spriteEffects == SpriteEffects.None) return 1;
            return 0;
        }
    }
}