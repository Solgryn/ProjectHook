using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectHook.GameFrameWork;
using ProjectHook.Menu;

namespace ProjectHook
{
    public static class Globals
    {
        public const float SMOOTH_SLOW = 0.05f;
        public const float SMOOTH_MEDIUM = 0.15f;
        public const float SMOOTH_FAST = 0.3f;

        public const string LAYER_SOLID = "Solid";
        public const string LAYER_BAD = "Bad";
        public const string LAYER_GOAL = "Goal";


        public static TitleScreen TitleScreen;
        public static StageSelect StageSelect;
        public static ResultScreen ResultScreen;
        
        //Descriptions are what is displayed in the menus
        public enum Levels
        {
            None,
            [Description("Level One")]
            Level1,
            [Description("Level Two")]
            Level2,
            [Description("Level Three")]
            Level3
        }

        public enum Menus
        {
            TitleScreen,
            StageSelect,
            ResultScreen
        }

        public static Vector2 GetControl(PlayerIndex index)
        {
            var control = new Vector2(0, 0);
            if (GamePad.GetState(index).IsConnected)
            {
                var gamepadState = GamePad.GetState(index);

                control = gamepadState.ThumbSticks.Left; //Control via stick

                //Control via DPad
                if (gamepadState.DPad.Right == ButtonState.Pressed)
                    control.X = 1;
                if (gamepadState.DPad.Left == ButtonState.Pressed)
                    control.X = -1;

                if (gamepadState.DPad.Down == ButtonState.Pressed)
                    control.Y = 1;
                if (gamepadState.DPad.Up == ButtonState.Pressed)
                    control.Y = -1;
            }
            else
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Left))
                    control.X = -1;
                if (Keyboard.GetState().IsKeyDown(Keys.Right))
                    control.X = 1;
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                    control.Y = -1;
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                    control.Y = 1;
            }

            control = Vector2.Clamp(control, new Vector2(-1, -1), new Vector2(1, 1));
            return control;
        }
    }
}
