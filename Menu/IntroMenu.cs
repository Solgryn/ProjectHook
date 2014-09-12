using System;
using System.Collections.Generic;
using System.Net.Mime;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectHook.GameFrameWork;

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
        }

        public override void Update(GameTime gameTime)
        {

            items[0].SpriteColor = Color.Green;

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                items[1].SpriteColor = Color.Green;
            }
        }
    }
}