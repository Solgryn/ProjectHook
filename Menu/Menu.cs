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
            //Move plus
            if (Control >= 1 && SelectCooldown.IsOff() && Selection < Items.Count - 1)
            {
                Selection++;
                SelectCooldown.GoOnCooldown();
            }

            //Move minus
            if (Control <= -1 && SelectCooldown.IsOff() && Selection > 0)
            {
                Selection--;
                SelectCooldown.GoOnCooldown();
            }

            SelectCooldown.Decrement();

            //Reset cooldown if no control is held
            if (Control == 0)
                SelectCooldown.Reset();
        }
    }
}
