using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using GrappleRace.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectHook.GameFrameWork;

namespace ProjectHook.Menu
{
    public class StageSelect : TextObject, IMenu
    {
        private readonly SpriteFont _font;
        private readonly GameHost _game;
        private readonly List<SpriteObject> _items = new List<SpriteObject>();
        public bool IsMenuOpen { get; set; }
        private int _selectedItem;

        private const string Folder = "StageSelect/";

        private readonly Cooldown _selectCooldown = new Cooldown(15);

        public enum States
        {
            Race,
            Options,
            Exit,
        }

        public States MenuState { get; set; }

        public StageSelect(GameHost game, SpriteFont font, Vector2 position) : base(game, font, position)
        {
            _font = font;
            _game = game;
        }

        public void ShowMenu()
        {
            _selectedItem = 0;
            IsMenuOpen = true;

            //Load textures
            var bgTex = Game.Content.Load<Texture2D>("StageSelect/Background");

            var stages = new List<Texture2D>
            {
                Game.Content.Load<Texture2D>(Folder + "Stage1"),
                Game.Content.Load<Texture2D>(Folder + "Stage2"),
                Game.Content.Load<Texture2D>(Folder + "Stage3")
            };

            var title = new SpriteObject(_game, new Vector2(32, 175), bgTex); //ATM, I don't know why these magic numbers work to center the image (35, 175)

            Game.GameObjects.Add(title);

            for (var i = 0; i < stages.Count; i++)
            {
                var menuItem = new SpriteObject(_game, new Vector2(0, 0), stages[i])
                {
                    SourceRect = new Rectangle(80, 0, 80, 64)
                };

                float width = menuItem.SourceRect.Width + 8;

                //Place objects
                var x = Camera.Width / 2f;
                x -= width * (stages.Count / 2f);
                x += width * i;
                var y = Camera.Height / 2f + 150;
                menuItem.Position = new Vector2(x, y);

                //Add objects
                _items.Add(menuItem);
                _game.GameObjects.Add(menuItem);
            }
        }

        public void CloseMenu()
        {
            _items.Clear();
        }

        public void OpenSelection()
        {
            switch (_items[_selectedItem].SpriteTexture.Name) //Get texture name
            {
                case Folder + "Stage1":
                    Game.GoToLevel(Globals.Levels.Level1);
                    break;
                case Folder + "Stage2":
                    Game.GoToLevel(Globals.Levels.Level2);
                    break;
                case Folder + "Stage3":
                    Game.GoToLevel(Globals.Levels.Level2); //TODO
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            var controlX = Globals.GetControl(PlayerIndex.One).X; //Get control

            if (controlX >= 1 && _selectCooldown.IsOff() && _selectedItem < _items.Count - 1)
            {
                _selectedItem++;
                _selectCooldown.GoOnCooldown();
            }

            if (controlX <= -1 && _selectCooldown.IsOff() && _selectedItem > 0)
            {
                _selectedItem--;
                _selectCooldown.GoOnCooldown();
            }

            _selectCooldown.Decrement();

            foreach (var item in _items)
            {
                item.SourceRect = new Rectangle(0, 0, 80, 64); //Reset all sprites
            }

            _items[_selectedItem].SourceRect = new Rectangle(80, 0, 80, 64); //Change selected item sprite

            if(Math.Abs(controlX) == 0)
                _selectCooldown.Reset();
        }
    }
}
