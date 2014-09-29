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
    public class TitleScreen : Menu<TextObject>, IMenu
    {
        private readonly List<String> _menuItems = new List<string> {"Race", "Exit"};
        private TextObject _menuitem;
        public bool IsMenuOpen { get; set; }
        private int _selectedItem;

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

        public void ShowMenu()
        {
            _selectedItem = 0;

            IsMenuOpen = true;
            var titleTex = Game.Content.Load<Texture2D>("TitleScreen/Background");

            var _title = new SpriteObject(_game, new Vector2(32, 175), titleTex); //ATM, I don't know why these magic numbers work to center the image (35, 175)

            Game.GameObjects.Add(_title);
            for (var i = 0; i < _menuItems.Count; i++)
            {
                _menuitem = new TextObject(_game, _font, new Vector2(0, 0), _menuItems[i]);
                //Centers the text
                float x = 768/2 - _menuitem.BoundingBox.Width/2;
                float y = 432/2 - _menuitem.BoundingBox.Height + i*30;
                _menuitem.Position = new Vector2(x, y);
                Items.Add(_menuitem);
                _game.GameObjects.Add(_menuitem);
            }

            Items[_selectedItem].SpriteColor = Color.Green;
        }

        public void OpenSelection()
        {
            switch (_menuItems[_selectedItem])
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
                item.SpriteColor = Color.White; //Reset all sprites
            }

            Items[Selection].SpriteColor = Color.Green;
        }
    }
}