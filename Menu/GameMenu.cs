using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using OpenTK.Graphics.ES20;
using ProjectHook.GameFrameWork;

namespace ProjectHook.Menu
{
    class GameMenu: TextObject, IMenu
    {
        private readonly SpriteFont _font;
        private readonly GameHost _game;
        private readonly List<String> menuItems = new List<string>() {"Resume Game", "Options", "Exit Game"};
        private SpriteObject _menu;
        private TextObject _menuitem;
        private List<TextObject> items = new List<TextObject>();
        private int selectedItem = 0;
        public bool isMenuOpen { get; set; }
        private bool _canPressKeyMenuDown = true;
        private bool _canPressKeyMenuUp = true;

        public enum MenuState
        {
            [Description("0")]
            Resume,
            [Description("1")]
            Options,
            [Description("2")]
            Exit
        }

        GameMenu.MenuState _menuState = new GameMenu.MenuState();
        public GameMenu.MenuState _MenuState { get { return _menuState; } set { _menuState = value; } }
        public GameMenu(GameHost game, SpriteFont font, Vector2 position) : base(game, font, position)
        {
            _font = font;
            _game = game;
        }

        public void CloseMenu()
        {
            isMenuOpen = false;
            int menuCount = menuItems.Count;
            int lastMenuItem = Game.GameObjects.IndexOf(_menuitem);
            Game.GameObjects.Remove(_menu);
            Game.GameObjects.RemoveRange(lastMenuItem - menuCount, menuCount);
            items.Clear();
            selectedItem = 0;
        }
        public void ShowMenu()
        {
            isMenuOpen = true;
            var menubg = Game.Content.Load<Texture2D>("menubg");
            _menu = new SpriteObject(_game, new Vector2(0, 0), menubg);
            _menu.SpriteColor = new Color(0, 0, 0, 150);
            Game.GameObjects.Add(_menu);
            for (int i = 0; i < menuItems.Count; i++)
            {
                _menuitem = new TextObject(_game, _font, new Vector2(0, 0), menuItems[i]);
                //Centers the text
                float x = 768 / 2 - _menuitem.BoundingBox.Width / 2;
                float y = 432 / 2 - _menuitem.BoundingBox.Height + i * 30;
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
                items[selectedItem - 1].SpriteColor = Color.White; // Throws an expection!!
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
