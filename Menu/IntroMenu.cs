using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Mime;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpenTK.Input;
using ProjectHook.GameFrameWork;
using Keyboard = Microsoft.Xna.Framework.Input.Keyboard;

namespace ProjectHook
{
    internal class IntroMenu : TextObject, IMenu
    {
        private readonly SpriteFont _font;
        private readonly GameHost _game;
        private readonly List<String> menuItems = new List<string> {"Single Player", "Two player", "Options", "Exit"};
        private List<TextObject> items = new List<TextObject>();
        private SpriteObject _menu;
        private TextObject _menuitem;
        private bool canPressKey;
        public bool isMenuOpen { get; set; }
        private int selectedItem = 0;
        private bool _canPressKeyMenuDown = true;
        private bool _canPressKeyMenuUp = true;

        public enum MenuState
        {
            [Description("0")]
            Single,
            [Description("1")]
            Multi,
            [Description("2")]
            Options,
            [Description("3")]
            Exit
        }

        MenuState _menuState = new MenuState();
        public MenuState _MenuState { get { return _menuState; } set { _menuState = value; } }

        private bool _canpressKeyMenuEnter = true;

        public IntroMenu(GameHost game, SpriteFont font, Vector2 position) : base(game, font, position)
        {
            _font = font;
            _game = game;
        }


        public void ShowMenu()
        {
            var menubg = Game.Content.Load<Texture2D>("menubg");
            _menu = new SpriteObject(_game, new Vector2(0, 0), menubg);
            _menu.SpriteColor = new Color(0, 0, 0, 150);
            Game.GameObjects.Add(_menu);
            for (int i = 0; i < menuItems.Count; i++)
            {
                _menuitem = new TextObject(_game, _font, new Vector2(0, 0), menuItems[i]);
                //Centers the text
                float x = 768/2 - _menuitem.BoundingBox.Width/2;
                float y = 432/2 - _menuitem.BoundingBox.Height + i*30;
                _menuitem.Position = new Vector2(x, y);
                items.Add(_menuitem);
                _game.GameObjects.Add(_menuitem);
            }
            items[selectedItem].SpriteColor = Color.Green;
        }

        public override void Update(GameTime gameTime)
        {
            

            if (Keyboard.GetState().IsKeyDown(Keys.Down) && _canPressKeyMenuDown && selectedItem != items.Count - 1)
            {
                _canPressKeyMenuDown = false;
                selectedItem++;
                items[selectedItem - 1].SpriteColor = Color.White;
                items[selectedItem].SpriteColor = Color.Green;
                _menuState++;

            }

            if (Keyboard.GetState().IsKeyUp(Keys.Down) && !_canPressKeyMenuDown)
            {
                _canPressKeyMenuDown = true;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up) && _canPressKeyMenuUp && selectedItem != 0)
            {
                _canPressKeyMenuUp = false;
                selectedItem--;
                items[selectedItem + 1].SpriteColor = Color.White;
                items[selectedItem].SpriteColor = Color.Green;
                _menuState--;
            }

            if (Keyboard.GetState().IsKeyUp(Keys.Up) && !_canPressKeyMenuUp)
            {
                _canPressKeyMenuUp = true;
            }

        }


    }
}