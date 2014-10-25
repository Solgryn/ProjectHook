using System;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectHook.GameFrameWork;

namespace ProjectHook
{
    public sealed class Player : SpriteObject
    {
        public readonly PlayerIndex PlayerIndex;
        private readonly int _playerIndexInt;
        private TextObject IndexText;

        public Vector2 Velocity = new Vector2(0, 0);
        public float Acceleration = 0.6f;
        public float Deacceleration = 0.4f;
        public float MaxSpeed = 3.5f;
        private float _startMaxSpeed;
        public float JumpStrength = -10f;
        public bool OnGround = false;
        public bool CanJump = true;
        public int Ammo = 3;

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

        //Cooldowns
        private readonly Cooldown _grappleCooldown = new Cooldown(40);

        //Debuffs
        private readonly Cooldown _slowDebuff = new Cooldown(150);

        //Buffs
        private readonly Cooldown _speedBuff = new Cooldown(150);

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
            _startMaxSpeed = MaxSpeed;

            OriginX = 32;
            OriginY = 32;

            switch (PlayerIndex)
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

            //Make the text above the players
            IndexText = new TextObject(Game, Game.Fonts["thefont"], new Vector2(0, 0));
            Game.GameObjects.Add(IndexText);
        }

        public override void Update(GameTime gameTime)
        {
            MaxSpeed = _startMaxSpeed;
            OnGround = false;

            #region Cooldowns and Debuffs
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

            //Slow debuff
            if (!_slowDebuff.IsOff())
            {
                SpriteColor = Color.IndianRed; //Turn the player red while they have the debuff
                MaxSpeed = _startMaxSpeed*0.6f; //Players are slower
                _slowDebuff.Decrement();
            }
            else if (SpriteColor == Color.IndianRed)
            {
                SpriteColor = Color.White;
            }

            //Speed buff
            if (!_speedBuff.IsOff())
            {
                SpriteColor = Color.LightGreen; //Turn the player green while they have the buff
                MaxSpeed = _startMaxSpeed*1.4f; //Players are faster
                _speedBuff.Decrement();
            }
            else if (SpriteColor == Color.LightGreen)
            {
                SpriteColor = Color.White;
            }
                
            #endregion

            #region Controls
            //Get X control
            var controlX = Globals.GetControl(PlayerIndex).X;

            //Get buttons
            if (GamePad.GetState(PlayerIndex).IsConnected)
            {
                var gamepadState = GamePad.GetState(PlayerIndex);
                _jumpKey = gamepadState.IsButtonDown(Buttons.A);
                _grappleKey = gamepadState.IsButtonDown(Buttons.B);
            }
            else
            {
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
            if (_jumpKey && CanJump && CanControl)
            {
                Sounds.PlaySound(Sounds.Jump);
                Velocity.Y = JumpStrength;
                CanJump = false;
            }
                //If jump key isn't held, shorten the jump
            else if(Velocity.Y < 0 && !_jumpKey && CanControl)
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
            //To avoid uneccessary calculations, velocity is checked for whether it's left/right/up/down

            //X
            var temp = Math.Abs(Velocity.X);
            var way = MathHelper.Clamp(Velocity.X, -1, 1);
            for (var i = 0; i < Math.Abs(Velocity.X); i++)
            {
                //Update hit detecors
                Detector.Left = new Rectangle((int)(PositionX - detectorXOffset), (int)(PositionY - detectorYOffset), 1, (int)detectorHeight);
                Detector.Right = new Rectangle((int)(PositionX + detectorXOffset), (int)(PositionY - detectorYOffset), 1, (int)detectorHeight);

                //Overlaps obstacles to the right
                if (Velocity.X > 0)
                {
                    if (OverlapsTileLayer(Detector.Right, Globals.LAYER_SOLID)) //Very costly calculation
                    {
                        Velocity.X = 0;
                        break;
                    }
                }

                //Overlaps obstacles to the left
                if (Velocity.X < 0)
                {
                    if (OverlapsTileLayer(Detector.Left, Globals.LAYER_SOLID)) //Very costly calculation
                    {
                        Velocity.X = 0;
                        break;
                    }
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

                

                //Hit the ceiling
                if (Velocity.Y < 0)
                {
                    if (OverlapsTileLayer(Detector.Up, Globals.LAYER_SOLID)) //Very costly calculation
                    {
                        Velocity.Y = 0;
                        break;
                    }
                }
                
                //Hit the ground
                if (Velocity.Y > 0)
                {
                    if (OverlapsTileLayer(Detector.Down, Globals.LAYER_SOLID)) //Very costly calculation
                    {
                        OnGround = true;
                        CanJump = true;
                        Velocity.Y = 0;
                        break;
                    }
                    //Is off the ground
                    OnGround = false;
                    CanJump = false;
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

            //Display font above head
            IndexText.Position = new Vector2(PositionX-Camera.Position.X-24, PositionY - 48);
            IndexText.Text = PlayerIndex + " (" + Ammo + ")"; //display the text (player number and ammo)

            #region Player dies
            //If out of frame, die
            if (IsOutOfFrame(256) || PositionX + 16 < Camera.Position.X || OverlapsTileLayer(BoundingBox, Globals.LAYER_BAD))
                Die();
            #endregion

            #region Interactables (Goal, Powerups)

            //Player reaches the goal
            if (OverlapsTileLayer(BoundingBox, Globals.LAYER_GOAL))
                Game.GoToMenu(Game.CurrentMenu);

            //Get ammo
            var ammoCrate = OverlapsTileType(BoundingBox, "ammo"); //Check if there's an ammo crate where you are
            if (ammoCrate != null) //If there is, get ammo and destroy the crate
            {
                Sounds.PlaySound(Sounds.Powerup);
                Collections.Tiles.Remove(ammoCrate);
                Ammo++;
            }

            //Get speedboost
            var speedCrate = OverlapsTileType(BoundingBox, "speed"); //Check if there's an ammo crate where you are
            if (speedCrate != null) //If there is, get ammo and destroy the crate
            {
                Sounds.PlaySound(Sounds.Powerup);
                Collections.Tiles.Remove(speedCrate);
                _speedBuff.GoOnCooldown();
            }

            //Goal
            if (OverlapsTileType(BoundingBox, "goal") != null)
            {
                Game.CurrentRace.FinishRace();
                Game.GoToMenu(Globals.ResultScreen);
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
            if(Ammo == 0) return;
            Sounds.PlaySound(Sounds.Hook);
            var hook = new Hook(Game, Position, Game.Content.Load<Texture2D>("grapple"), this, 15);
            Game.GameObjects.Add(hook);
            Ammo--;
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
            Sounds.PlaySound(Sounds.Pull);
            //Set velocity of player 
            Velocity.Y = hook.Pull.Y * 2;
            Velocity.X = (hook.Pull.X * hook.GetDirection()) * 1.3f;

            //Player can't control for a little bit
            CantControlFor = 15;
        }

        public void Die()
        {
            Sounds.PlaySound(Sounds.Died);
            Position = new Vector2(Camera.Position.X + Camera.Width / 4f, Camera.Position.Y+100);
            Velocity = Vector2.Zero;
            _slowDebuff.GoOnCooldown(); //Add slow debuff
            _speedBuff.Reset(); //Remove speed buff
        }
    }
}