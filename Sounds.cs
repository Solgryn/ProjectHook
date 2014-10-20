using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;

namespace ProjectHook
{
    public static class Sounds
    {
        public static float MusicVolume = 1.0f;
        public static float MusicPitch = 0;
        public static float MusicPan = 0;

        public static float SoundEffectVolume = 0.25f;

        //Music
        public static SoundEffectInstance Music1;
        public static SoundEffectInstance Music2;
        public static SoundEffectInstance Music3;
        public static SoundEffectInstance ResultScreen;

        //Sound effects
        public static SoundEffectInstance Died;
        public static SoundEffectInstance Jump;
        public static SoundEffectInstance Powerup;
        public static SoundEffectInstance Select; //When selecting in the menu
        public static SoundEffectInstance Hook;
        public static SoundEffectInstance Pull;
    }
}
