using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Mime;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpenTK.Input;
using ProjectHook.GameFrameWork;
using ProjectHook.Menu;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace ProjectHook.Menu
{
    public class TitleScreen : Menu<FontRenderer>, IMenu
    {
        private readonly List<String> _menuItems = new List<string> {"Race", "Exit"};
        public bool IsMenuOpen { get; set; }

        public enum States
        {
            Race,
            Options,
            Exit,
        }

        public States MenuState { get; set; }

        public TitleScreen(GameHost game, SpriteFont font, Vector2 position) : base(game, font, position)
        {
            _font = font;
            _game = game;
        }

        public Globals.Menus GetName()
        {
            return Globals.Menus.TitleScreen;
        }

        public void ShowMenu()
        {
            IsMenuOpen = true;
            var titleTex = Game.Content.Load<Texture2D>("TitleScreen/Background");

            var _title = new SpriteObject(_game, new Vector2(0, 0), titleTex);

            Game.GameObjects.Add(_title);

            for (var i = 0; i < _menuItems.Count; i++)
            {
                var menuitem = Game.Content.NewFont("bitmapfont", new Vector2(Camera.Width / 2f, 200), FontRenderer.FontDisplays.Center);
                menuitem.Text = _menuItems[i];
                //Centers the text
                var x = Camera.Width / 2f;
                var y = Camera.Height / 2f + i * MenuSpace;
                menuitem.Position = new Vector2(x, y);
                menuitem.DoSineWave = true;
                Items.Add(menuitem);
                Collections.Fonts.Add(menuitem);
            }
        }

        public void OpenSelection()
        {
            switch (_menuItems[Selection])
            {
                case "Race":
                    Game.GoToMenu(Globals.StageSelect);
                    break;
                case "Exit":
                    Game.Exit();
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            Control = Globals.GetControl(PlayerIndex.One).Y; //Get control

            SelectionUpdate();

            foreach (var item in Items)
            {
                if (!item.Equals(Items[Selection]))
                {
                    item.SineWaveLength = item.SineWaveLength.SmoothTowards(2f, Globals.SMOOTH_FAST);
                    item.CharacterSpacing = item.CharacterSpacing.SmoothTowards(0, Globals.SMOOTH_FAST); ; //Reset all sprites if not selection
                }
            }

            Items[Selection].SineWaveLength = Items[Selection].SineWaveLength.SmoothTowards(12f, Globals.SMOOTH_FAST);
            Items[Selection].CharacterSpacing = Items[Selection].CharacterSpacing.SmoothTowards(15f, Globals.SMOOTH_FAST);
        }
    }
}