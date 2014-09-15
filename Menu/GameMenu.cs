using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        public GameMenu(GameHost game, SpriteFont font, Vector2 position) : base(game, font, position)
        {
            _font = font;
            _game = game;
        }

        public void CloseMenu()
        {
            int menuCount = menuItems.Count;
            int lastMenuItem = Game.GameObjects.IndexOf(_menuitem);
            Game.GameObjects.Remove(_menu);
            Game.GameObjects.RemoveRange(lastMenuItem - menuCount, menuCount);
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
            
            
        }

    }
}
