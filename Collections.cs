using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using ProjectHook.GameFrameWork;

namespace ProjectHook
{
    public static class Collections
    {
        public static List<Player> Players = new List<Player>();
        public static List<Tile> Tiles = new List<Tile>();
        public static List<FontRenderer> Fonts = new List<FontRenderer>();
        public static List<SoundEffectInstance> Music = new List<SoundEffectInstance>();
        public static List<SoundEffectInstance> SoundEffects = new List<SoundEffectInstance>();
    }
}
