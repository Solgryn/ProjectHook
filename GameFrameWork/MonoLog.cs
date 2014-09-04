using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GrappleRace.GameFrameWork
{
    public class MonoLog : SpriteObject
    {
        //-------------------------------------------------------------------------------------
        // Class variables

        private List<TextString> TextStrings = new List<TextString>();
        private int _screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        private int _indentationLeft = 25;

        //-------------------------------------------------------------------------------------
        // Class constructors
        public MonoLog(GameHost game)
            : base(game)
        {
            // Set the default scale and color
            ScaleX = 1;
            ScaleY = 1;
            SpriteColor = Color.White;
            
            Speed = 0.8f;
        }

        public MonoLog(GameHost game, SpriteFont font)
            : this(game)
        {
            Font = font;
            LineSpacing = Font.LineSpacing;
        }

        public MonoLog(GameHost game, SpriteFont font, Color fontColor)
            : this(game, font)
        {
            FontColor = fontColor;
        }



        //-------------------------------------------------------------------------------------
        // Properties

        public SpriteFont Font { get; set; }

        public Color FontColor { get; set; }

        public float Speed { get; set; }

        public int LineSpacing { get; set; }

        //-------------------------------------------------------------------------------------
        // Game functions

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Font != null && TextStrings.Count > 0)
            {
                for (int i = 0; i < TextStrings.Count; i++)
                {
                    if (TextStrings[i].Position < _screenHeight)
                        spriteBatch.DrawString(Font, TextStrings[i].Text, new Vector2(_indentationLeft, TextStrings[i].Position), FontColor);

                    if (TextStrings[i].Position < -50)
                        TextStrings.Remove(TextStrings[i]);
                }
                // Draw the text
                //spriteBatch.DrawString(Font, Text, Position, SpriteColor, Angle, Origin, Scale, SpriteEffects.None, LayerDepth);
            }
            
           // spriteBatch.DrawString(Font, "MY LOG", new Vector2(200,200), Color.White );

        }

        public void Write(String message)
        {
            if (TextStrings.Count == 0)
            {
                TextStrings.Add(new TextString(message, _screenHeight));
            }

            else
            {
                TextString ts = new TextString(message, TextStrings.Last().Position+LineSpacing);
                TextStrings.Add(ts);    
            }
            
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var textString in TextStrings)
            {
                textString.Position-=Speed;
            }
            base.Update(gameTime);
        }

        private class TextString
        {
            public TextString(String text, float position)
            {
                Text = text;
                Position = position;
            }

            public float Position{ get; set; }
            public String Text{ get; set; }
        }
    }

}
