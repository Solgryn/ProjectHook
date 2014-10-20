using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectHook.GameFrameWork
{
    public class FontRenderer
    {
        public string Text;
        public Vector2 Position;
        public FontDisplays FontDisplay = FontDisplays.Right;
        public float CharacterSpacing;
        public bool DoSineWave;
        private double _timer = 0;
        public float SineWaveLength = 0;

        public enum FontDisplays
        {
            Right,
            Center
        }

        public FontRenderer(FontFile fontFile, Texture2D fontTexture, Vector2 position, FontDisplays fontDisplay)
        {
            CharacterSpacing = 0;
            _fontFile = fontFile;
            _texture = fontTexture;
            _characterMap = new Dictionary<char, FontChar>();
            Position = position;
            FontDisplay = fontDisplay;

            foreach (var fontCharacter in _fontFile.Chars)
            {
                var c = (char)fontCharacter.ID;
                _characterMap.Add(c, fontCharacter);
            }
        }

        internal class Draw
        {
            public Texture2D Texture;
            public Vector2 Position;
            public Rectangle SourceRectangle;
            public Color Color;

            public Draw(Texture2D texture, Vector2 position, Rectangle sourceRectangle, Color color)
            {
                Texture = texture;
                Position = position;
                SourceRectangle = sourceRectangle;
                Color = color;
            }
        }

        private Dictionary<char, FontChar> _characterMap;
        private FontFile _fontFile;
        private Texture2D _texture;
        public void DrawText(SpriteBatch spriteBatch)
        {
            if (Text == null) return; //If no text, return

            float offsetX = 0;
            var draws = new List<Draw>();
            var dx = Position.X;
            var dy = Position.Y;
            float pixelLength = 0;
            foreach (var c in Text)
            {
                FontChar fc;
                if (_characterMap.TryGetValue(c, out fc))
                {
                    var sourceRectangle = new Rectangle(fc.X, fc.Y, fc.Width, fc.Height);
                    var position = new Vector2(dx + fc.XOffset, dy + fc.YOffset);

                    draws.Add(new Draw(_texture, position, sourceRectangle, Color.White)); //Add the current character to draw later

                    dx += fc.XAdvance + CharacterSpacing;
                    //Gradually find out the width of all the characters in the string
                    //by adding ther width of the current character, optional character specing and 1 (which is the space between each character)
                    pixelLength += fc.Width + CharacterSpacing + 1;
                }
            }
            if (FontDisplay == FontDisplays.Center) //If the text should be centered then
            {
                offsetX = (float)Math.Round(pixelLength * -0.5f); //Add half of the pixel length to the string, which centers it
            }
            _timer += 0.05;
            foreach (var draw in draws)
            {
                var characterYOffset = 0f;
                if (DoSineWave)
                    characterYOffset = (float)Math.Sin(_timer + draw.Position.X / 5f) * SineWaveLength; 
                //Draw all the "saved" characters, with the offset if necessary, also offsetting with character spacing
                spriteBatch.Draw(draw.Texture, new Vector2(draw.Position.X + offsetX, draw.Position.Y + characterYOffset), draw.SourceRectangle, draw.Color);
            }
            draws.Clear();
        }
    }
}
