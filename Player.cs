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
        private double AnimationSpeed = 10;
        private int currentFrame = 0;
        
        public Player(GameHost game, Vector2 position, Texture2D texture)
            : base(game, position, texture)
        {
            SpriteTexture = texture;
            
        }

        public override void Update(GameTime gameTime)
        {
            var thumbsticks = GamePad.GetState(PlayerIndex.One).ThumbSticks;
            //Controls
            
            //PositionX = GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X * 3-;
            
            //Left
            if (Keyboard.GetState().IsKeyDown(Keys.Left) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X < 0)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
                //Velocity.X = Math.Max(Velocity.X - Acceleration, MaxSpeed*-1);
                Velocity.X = MaxSpeed * thumbsticks.Left.X;
            }
            else if (Velocity.X < 0)
            {
                
                Velocity.X = Math.Min(Velocity.X + Deacceleration, 0);
            }

            //Right
            if (Keyboard.GetState().IsKeyDown(Keys.Right) || GamePad.GetState(PlayerIndex.One).ThumbSticks.Left.X > 0)
            {
                spriteEffects = SpriteEffects.None;
                //Velocity.X = Math.Min(Velocity.X + Acceleration, MaxSpeed);
                Velocity.X = MaxSpeed*thumbsticks.Left.X;
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
                Velocity.Y *=0.9f;
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
            int[] running = {0, 1, 0, 2};

            AnimationCounter += AnimationSpeed;
            if (AnimationCounter == 100)
            {
                AnimationCounter = 0;
                currentFrame++;
                if (currentFrame == running.Length)
                {
                    currentFrame = 0;
                }
            }
            SourceRect = new Rectangle(0, running[currentFrame]*64, 64, 64);

        }
    }
}
