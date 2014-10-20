using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectHook.GameFrameWork;

namespace ProjectHook.Menu
{
    public class Menu<T> : TextObject
    {
        internal int MenuSpace = 64;
        internal readonly List<T> Items = new List<T>();
        internal SpriteFont _font;
        internal GameHost _game;
        internal readonly Cooldown SelectCooldown = new Cooldown(15);
        internal int Selection;
        internal float Control;

        public Menu(GameHost game, SpriteFont font, Vector2 position) : base(game, font, position)
        {
            _font = font;
            _game = game;
        }

        public virtual void CloseMenu()
        {
            Items.Clear();
        }

        public virtual void SelectionUpdate()
        {
            //move selection
            if (Math.Abs(Control) > 0.5f && SelectCooldown.IsOff())
            {
                var direction = Convert.ToInt16(Math.Round(Control)); //Returns either -1 or 1

                if(direction > 0 && Selection < Items.Count - 1 ||
                   direction < 0 && Selection > 0) //Checks if selection is within boundaries
                {
                    Game.PlaySound(Sounds.Select);
                    Selection += direction; //Returns either -1 or 1
                    Selection = MathHelper.Clamp(Selection, 0, Items.Count - 1); //Adds the control to the selection, and keeps it within the menu range
                    SelectCooldown.GoOnCooldown();
                }
            }

            SelectCooldown.Decrement();

            //Reset cooldown if no control is held
            if (Math.Abs(Control) < 0.5f)
                SelectCooldown.Reset();
        }
    }
}
