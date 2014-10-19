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
    public class Results : Menu<TextObject>, IMenu
    {
        //TODO remove this redundancy, and make text bitmap text
        private readonly List<String> _menuItems = new List<string> { "Go Back"};
        private TextObject _menuitem;
        private int _selectedItem;

        private const string Folder = "Results/";

        public Results(GameHost game, SpriteFont font, Vector2 position) : base(game, font, position)
        {
            _font = font;
            _game = game;
        }

        public void ShowMenu()
        {
            Selection = 0;

            //Load textures
            var bgTex = Game.Content.Load<Texture2D>(Folder + "Background");

            //Add objects
            var title = new SpriteObject(_game, new Vector2(0, 0), bgTex);
            Game.GameObjects.Add(title);

            var ResultText = Game.Content.NewFont("bitmapfont", new Vector2(Camera.Width/2f, 200), FontRenderer.FontDisplays.Center);
            Collections.Fonts.Add(ResultText);

            ResultText.Text = "Player " + _game.CurrentRace.FirstPlace + " won!\n";

            for (var i = 0; i < _menuItems.Count; i++)
            {
                _menuitem = new TextObject(_game, _font, new Vector2(0, 0), _menuItems[i]);
                //Centers the text
                float x = Camera.Width / 2 - _menuitem.BoundingBox.Width / 2;
                float y = Camera.Height / 2 - _menuitem.BoundingBox.Height + i * 30;
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
                case "Go Back":
                    Game.GoToMenu(Globals.TitleScreen);
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
