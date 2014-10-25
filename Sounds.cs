using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using ProjectHook.GameFrameWork;

namespace ProjectHook
{
    public static class Sounds
    {
        private static GameHost _game;
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

        //Levels
        public static SoundEffectInstance Died;
        public static SoundEffectInstance Jump;
        public static SoundEffectInstance Powerup;
        public static SoundEffectInstance Hook;
        public static SoundEffectInstance Pull;

        //Menus
        public static SoundEffectInstance Select; //When pressing up/down in the menu
        public static SoundEffectInstance Confirm; //When pressing enter in the menu

        public static SoundEffectInstance CountdownTick; //Every second when the countdown ticks
        public static SoundEffectInstance CountdownDone; //When countdown is done

        public static void PlaySound(SoundEffectInstance sound, bool doLoop = false)
        {
            sound.Volume = SoundEffectVolume;
            sound.IsLooped = doLoop; //Should the music loop or not
            sound.Play(); //Starts the sound effect
        }
    }
}
