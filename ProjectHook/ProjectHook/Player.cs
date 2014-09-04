using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ProjectHook
{
    class Player : SpriteObject
    {
        
        public Vector2 Velocity = new Vector2(0, 0);
        public float Acceleration = 1.5f;
        public float Deacceleration = 0.8f;
        public float MaxSpeed = 3.5f;
        public float JumpStrength = -10f;
        public bool OnGround = false;
        private double _milisecondsSinceLastFrameUpdate = 0;
        private double AnimationCounter = 0;
        private double AnimationSpeed = 15;
        private int currentFrame = 0;

        //Animations
        public int[] CurrentAnimation = Animation.Idle;

        internal class Animation
        {
            public static readonly int[] Idle = {0};
            public static readonly int[] Running = {0, 1, 0, 2};
        }

        public Player(GameHost game, Vector2 position, Texture2D texture)
            : base(game, position, texture)
        {
            SpriteTexture = texture;
            
        }

        public override void Update(GameTime gameTime)
        {
            var thumbsticks = GamePad.GetState(PlayerIndex.One).ThumbSticks;

            //Controls
            
            //Left
            if (Keyboard.GetState().IsKeyDown(Keys.Left) || thumbsticks.Left.X < 0)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
                //Velocity.X = MaxSpeed * thumbsticks.Left.X;
                Velocity.X = MaxSpeed*-1;
            }
            else if (Velocity.X < 0)
            {
                Velocity.X = Math.Min(Velocity.X + Deacceleration, 0);
            }

            //Right
            if (Keyboard.GetState().IsKeyDown(Keys.Right) || thumbsticks.Left.X > 0)
            {
                spriteEffects = SpriteEffects.None;
                //Velocity.X = MaxSpeed*thumbsticks.Left.X;
                Velocity.X = MaxSpeed;
            }
            else if (Velocity.X > 0)
            {
                Velocity.X = Math.Max(Velocity.X - Deacceleration, 0);
            }

            //Jump
            if (((Keyboard.GetState().IsKeyDown(Keys.Z) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A)) && OnGround))
            {

                Velocity.Y = JumpStrength;
                OnGround = false;
                
            }
            else if(Velocity.Y < 0 && !(Keyboard.GetState().IsKeyDown(Keys.Z) || GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.A)))
            {
                Velocity.Y *=0.85f;
            }

            //Gravity
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
                    continue;
                }
                if (temp < 1)
                {
                    
                    PositionY+=temp*way;
                    break;
                }
                PositionY+=way;
                temp--;
            }

            //Set animations
            if ((Velocity.X > 0.5f || Velocity.X < -0.5f) && OnGround) ChangeAnimation(Animation.Running);
            if (Velocity.X > -0.5f && Velocity.X < 0.5f && OnGround) ChangeAnimation(Animation.Idle);
            
            //Animations
            AnimationCounter += AnimationSpeed;
            if (AnimationCounter >= 100)
            {
                AnimationCounter = 0;
                currentFrame++;
                if (currentFrame == CurrentAnimation.Length)
                    currentFrame = 0;
            }
            SourceRect = new Rectangle(0, CurrentAnimation[currentFrame]*64, 64, 64);
        }

        public void ChangeAnimation(int[] animation)
        {
            if (CurrentAnimation == animation) return;
            CurrentAnimation = Animation.Running;
            currentFrame = 0;
        }
    }
}
