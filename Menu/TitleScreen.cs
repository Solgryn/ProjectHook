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

namespace ProjectHook
{
    internal class TitleScreen : TextObject, IMenu
    {
        private readonly SpriteFont _font;
        private readonly GameHost _game;
        private readonly List<String> _menuItems = new List<string> {"Race", "Options", "Exit"};
        private readonly List<TextObject> _items = new List<TextObject>();
        private SpriteObject _menu;
        private TextObject _menuitem;
        public bool IsMenuOpen { get; set; }
        private int _selectedItem;
        private bool _canPressKeyMenuDown = true;
        private bool _canPressKeyMenuUp = true;

        internal enum States
        {
            Race,
            Options,
            Exit,
        }

        States _menuState;
        public States _MenuState { get { return _menuState; } set { _menuState = value; } }

        public TitleScreen(GameHost game, SpriteFont font, Vector2 position) : base(game, font, position)
        {
            _font = font;
            _game = game;
        }

        public void ShowMenu()
        {
            IsMenuOpen = true;
            var menubg = Game.Content.Load<Texture2D>("menubg");
            _menu = new SpriteObject(_game, new Vector2(0, 0), menubg);
            _menu.SpriteColor = new Color(0, 0, 0, 150);
            Game.GameObjects.Add(_menu);
            for (int i = 0; i < _menuItems.Count; i++)
            {
                _menuitem = new TextObject(_game, _font, new Vector2(0, 0), _menuItems[i]);
                //Centers the text
                float x = 768/2 - _menuitem.BoundingBox.Width/2;
                float y = 432/2 - _menuitem.BoundingBox.Height + i*30;
                _menuitem.Position = new Vector2(x, y);
                _items.Add(_menuitem);
                _game.GameObjects.Add(_menuitem);
            }
            _items[_selectedItem].SpriteColor = Color.Green;
        }

        public void OpenSelection()
        {
            switch (_menuItems[_selectedItem])
            {
                case "Race":
                    Game.GoToLevel(Globals.Levels.Level1);
                    break;
                case "Exit":
                    Game.Exit();
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            var controlY = Globals.GetControl(PlayerIndex.One).Y;

            if (controlY == 1 && _canPressKeyMenuDown && _selectedItem != _items.Count - 1)
            {
                _canPressKeyMenuDown = false;
                _selectedItem++;
                _items[_selectedItem - 1].SpriteColor = Color.White; // Throws an expection!!
                _items[_selectedItem].SpriteColor = Color.Green;

            }

            if (controlY == 0 && !_canPressKeyMenuDown)
            {
                _canPressKeyMenuDown = true;
            }

            if (controlY == -1 && _canPressKeyMenuUp && _selectedItem != 0)
            {
                _canPressKeyMenuUp = false;
                _selectedItem--;
                _items[_selectedItem + 1].SpriteColor = Color.White;
                _items[_selectedItem].SpriteColor = Color.Green;
            }

            if (controlY == 0 && !_canPressKeyMenuUp)
            {
                _canPressKeyMenuUp = true;
            }
        }
    }
}