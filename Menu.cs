using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectHook.GameFrameWork;

namespace ProjectHook
{
    class Menu : TextObject
    {
        private SpriteFont _font;
        private GameHost _game;
        private SpriteObject _menu;
        private TextObject _menuitem;
        private List<String> menuItems = new List<string>() {"Single Player", "Two player", "Options", "Exit"};
        private bool isMenuOpen = false;
        private bool canPressKey;

        public Menu(GameHost game, SpriteFont font, Vector2 position) : base(game, font, position)
        {
            _font = font;
            _game = game;
        }

        public void ShowMenu()
        {
            for (int i = 0; i < menuItems.Count; i++)
            {
                _menuitem = new TextObject(_game, _font, new Vector2(0, 0), menuItems[i]);
                //Centers the text
                float x = 768/2 - _menuitem.BoundingBox.Width/2;
                float y = 432/2 - _menuitem.BoundingBox.Height + i*30;
                _menuitem.Position = new Vector2(x,y);
                _game.GameObjects.Add(_menuitem);

            }

            var menubg = Game.Content.Load<Texture2D>("menubg");
            _menu = new SpriteObject(_game, new Vector2(0,0), menubg);
            _menu.SpriteColor = new Color(0, 0, 0, 50);
            Game.GameObjects.Add(_menu);
        }

        public override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.F4) && !isMenuOpen && canPressKey)
            {
                isMenuOpen = true;
                canPressKey = false;

                ShowMenu();
                _menu.Position = Camera.Position;

            }


            if (Keyboard.GetState().IsKeyDown(Keys.F4) && isMenuOpen && canPressKey)
            {
                canPressKey = false;
                isMenuOpen = false;

                Game.GameObjects.Remove(_menu);
                int menuCount = menuItems.Count;
                int lastMenuItem = Game.GameObjects.IndexOf(_menuitem);
                Game.GameObjects.RemoveRange(lastMenuItem - menuCount, menuCount + 1);


            }
            if (Keyboard.GetState().IsKeyUp(Keys.F4))
                canPressKey = true;
            

        }




    }
}
