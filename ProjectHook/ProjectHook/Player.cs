using System;
using System.Diagnostics;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectHook
{
    public sealed class Player : SpriteObject
    {
        //Hej :D
        private readonly PlayerIndex _playerIndex;
        private int _playerIndexInt;

        public Vector2 Velocity = new Vector2(0, 0);
        public float Acceleration = 0.6f;
        public float Deacceleration = 0.4f;
        public float MaxSpeed = 3.5f;
        public float JumpStrength = -10f;
        public bool OnGround = false;
        public bool canJump = true;

        private double _animationCounter;
        private double _animationSpeed;
        private int _currentFrame;

        internal class Detector
        {
            public static Rectangle Left;
            public static Rectangle Right;
            public static Rectangle Down;
            public static Rectangle Up;

            public static readonly float XOffset = 16;
            public static readonly float YOffset = 16;
            public static readonly int Width = 32;
            public static readonly int Height = 47;
        }


        public bool CanControl = true;
        public int CantControlFor;

        private bool _jumpKey;
        private bool _grappleKey;
        private readonly Cooldown _grappleCooldown = new Cooldown(40);

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
            _playerIndex = playerIndex;

            OriginX = 32;
            OriginY = 32;

            switch (_playerIndex)
            {
                case PlayerIndex.One:
                    _playerIndexInt = 0;
                    break;
                case PlayerIndex.Two:
                    _playerIndexInt = 1;
                    break;
                default:
                    _playerIndexInt = 0;
                    break;
            }
            BoundingBox = new Rectangle(16, 16, 32, 48);
        }

        public override void Update(GameTime gameTime)
        {
            OnGround = false;

            #region Cooldowns
            //Manage cooldowns
            _grappleCooldown.Decrement();
            //Can't control for x frames
            if (CantControlFor > 0)
            {
                CanControl = false;
                CantControlFor--;
            }
            else
            {
                CanControl = true;
            }
                
            #endregion

            #region Controls
            //Use either gamepad or keyboard for input
            float controlX;
            if (GamePad.GetState(_playerIndex).IsConnected)
            {
                controlX = GamePad.GetState(_playerIndex).ThumbSticks.Left.X;
                _jumpKey = GamePad.GetState(_playerIndex).IsButtonDown(Buttons.A);
                _grappleKey = GamePad.GetState(_playerIndex).IsButtonDown(Buttons.B);
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

            //Walk Left
            if (controlX < 0 && CanControl && Velocity.X > -MaxSpeed)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
                Velocity.X = Math.Max(Velocity.X - Acceleration, controlX * MaxSpeed);
            }
                //Deccelerate to the left
            else if (Velocity.X < 0)
            {
                Velocity.X = Math.Min(Velocity.X + Deacceleration, 0);
            }

            //Walk Right
            if (controlX > 0 && CanControl && Velocity.X < MaxSpeed)
            {
                spriteEffects = SpriteEffects.None;
                Velocity.X = Math.Min(Velocity.X + Acceleration, controlX * MaxSpeed);
            }
                //Deccelerate to the right
            else if (Velocity.X > 0)
            {
                Velocity.X = Math.Max(Velocity.X - Deacceleration, 0);
            }

            //Jump
            if (_jumpKey && canJump && CanControl)
            {
                Velocity.Y = JumpStrength;
                canJump = false;
            }
                //If jump key isn't held, shorten the jump
            else if(Velocity.Y < 0 && !_jumpKey)
            {
                Velocity.Y *=0.85f;
            }
            
            //Shoot Hook
            if (_grappleKey && _grappleCooldown.IsOff() && CanControl)
            {
                ThrowHook();
                _grappleCooldown.GoOnCooldown();
            }
            #endregion

            #region Moving the player

            var detectorXOffset = Detector.XOffset*ScaleX;
            var detectorYOffset = Detector.YOffset*ScaleY;
            var detectorWidth = Detector.Width*ScaleX;
            var detectorHeight = Detector.Height*ScaleY;

            //Apply Gravity
            if(!OnGround)
                Velocity.Y += 0.35f;

            //Here we move the player 1 pixel at a time, and check for collisions per pixel

            //X
            var temp = Math.Abs(Velocity.X);
            var way = MathHelper.Clamp(Velocity.X, -1, 1);
            for (var i = 0; i < Math.Abs(Velocity.X); i++)
            {
                //Update hit detecors
                Detector.Left = new Rectangle((int)(PositionX - detectorXOffset), (int)(PositionY - detectorYOffset), 1, (int)detectorHeight);
                Detector.Right = new Rectangle((int)(PositionX + detectorXOffset), (int)(PositionY - detectorYOffset), 1, (int)detectorHeight);

                //Overlaps obstacles
                if (OverlapsSolid(Detector.Right) && Velocity.X > 0 ||
                    OverlapsSolid(Detector.Left) && Velocity.X < 0)
                {
                    Velocity.X = 0;
                    break;
                }
                    
                //If movement is less than 1, move the rest
                if (temp < 1)
                {
                    PositionX += temp*way;
                    break;
                }
                //Move a pixel
                PositionX+=way;
                temp--;
            }

            //Y
            temp = Math.Abs(Velocity.Y);
            way = MathHelper.Clamp(Velocity.Y, -1, 1);
            for (var i = 0; i < Math.Abs(Velocity.Y); i++)
            {
                //Update hit detectors
                Detector.Up = new Rectangle((int)(PositionX - detectorXOffset + 2), (int)(PositionY - 16f), (int)detectorWidth - 4, 1);
                Detector.Down = new Rectangle((int)(PositionX - detectorXOffset + 2), (int)(PositionY + 31), (int)detectorWidth - 4, 1);

                //Is off the ground
                if (!OverlapsSolid(Detector.Down))
                    canJump = false;

                //Hit the ceiling
                if (OverlapsSolid(Detector.Up) && Velocity.Y < 0)
                {
                    Velocity.Y = 0;
                    break;
                }

                //Hit the ground
                if (OverlapsSolid(Detector.Down) && Velocity.Y > 0)
                {
                    OnGround = true;
                    canJump = true;
                    Velocity.Y = 0;
                    break;
                }
                //If movement is less than 1, move the rest
                if (temp < 1)
                {
                    PositionY+=temp*way;
                    break;
                }
                //Move a pixel
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

            //Update texture image
            SourceRect = new Rectangle(64 * _playerIndexInt, (CurrentAnimation.Frames[_currentFrame] * 64) + 1, 64, 64);
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
            var hook = new Hook(Game, Position, Game.Content.Load<Texture2D>("grapple"), this, 15);
            Game.GameObjects.Add(hook);
        }

        //Player gets hit by hook
        public void GetHit(Hook hook, Player player)
        {
            var hookDirection = hook.GetDirection(); //Get direction of hook (either -1 or 1)

            //Set velocities of players depending on the direction of the hook
            Velocity.Y = hook.Pull.Y;
            Velocity.X = (hook.Pull.X * hookDirection ) * -1f;
            player.Velocity.X = hook.Pull.X * hookDirection;
            player.Velocity.Y = hook.Pull.Y;
            player.OnGround = false;
            OnGround = false;

            //Player can't control for a little bit
            CantControlFor = 20;
        }

        //Hook hits tile
        public void HookHit(Hook hook)
        {
            //Set velocity of player 
            Velocity.Y = hook.Pull.Y * 2;
            Velocity.X = (hook.Pull.X * hook.GetDirection()) * 1.5f;

            //Player can't control for a little bit
            CantControlFor = 15;
        }

        public bool OverlapsSolid(Rectangle hitbox)
        {
            foreach (var tile in Collections.Tiles)
            {
                if (hitbox.Intersects(tile.BoundingBox) && tile.LayerName == "Solid")
                    return true;
            }
            return false;
        }
    }
}