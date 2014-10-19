using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ProjectHook.GameFrameWork;

namespace ProjectHook
{
    public static class ExtensionMethods
    {
        public static float SmoothTowards(this float from, float to, float coefficient)
        {
            return from + (to - from)*coefficient;
        }

        //Get description attribute of enums
        public static string ToDescription(this Enum value)
        {
            var da = (DescriptionAttribute[])(value.GetType().GetField(value.ToString())).GetCustomAttributes(typeof(DescriptionAttribute), false);
            return da.Length > 0 ? da[0].Description : value.ToString();
        }

        //Add a new bitmap text, uses code from http://www.craftworkgames.com/blog/tutorial-bmfont-rendering-with-monogame/
        public static FontRenderer NewFont(this ContentManager content, string name, Vector2 position, FontRenderer.FontDisplays fontDisplay)
        {
            var fontFilePath = Path.Combine(content.RootDirectory, name + ".fnt");
            var fontFile = FontLoader.Load(fontFilePath);
            var fontTexture = content.Load<Texture2D>(name + "_0.png");

            return new FontRenderer(fontFile, fontTexture, position, fontDisplay);
        }
    }
}
