using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectHook.GameFrameWork;
using ProjectHook.Menu;

namespace ProjectHook
{
    public static class Globals
    {
        public const string LAYER_SOLID = "Solid";
        public const string LAYER_BAD = "Bad";
        public const string LAYER_GOAL = "Goal";


        public static TitleScreen TitleScreen;
        public static StageSelect StageSelect;
        public static Results ResultScreen;

        public enum Levels
        {
            None,
            TitleScreen,
            PauseMenu,
            [Description("Level1")]
            Level1,
            [Description("Level2")]
            Level2,
            [Description("Level3")]
            Level3,
            StageSelect
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
