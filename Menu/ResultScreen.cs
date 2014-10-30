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
    public class ResultScreen : Menu<FontRenderer>, IMenu
    {
        //TODO remove this redundancy
        private readonly List<String> _menuItems = new List<string> { "Go Back", "Re-Race" };

        private const string Folder = "Results/";

        public ResultScreen(GameHost game, SpriteFont font, Vector2 position) : base(game, font, position)
        {
            _font = font;
            _game = game;
        }

        public Globals.Menus GetName()
        {
            return Globals.Menus.ResultScreen;
        }

        public void ShowMenu()
        {
            Selection = 0;

            //Load textures
            var bgTex = Game.Content.Load<Texture2D>(Folder + "Background");

            //Add objects
            var title = new SpriteObject(_game, new Vector2(0, 0), bgTex);
            Game.GameObjects.Add(title);

            var ResultText = Game.Content.NewFont("bitmapfont", new Vector2(Camera.Width/2f, 260), FontRenderer.FontDisplays.Center);
            Collections.Fonts.Add(ResultText);

            ResultText.Text = "Player " + _game.CurrentRace.FirstPlace + " won!\n";

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
                case "Go Back":
                    Game.GoToMenu(Globals.TitleScreen);
                    break;
                case "Re-Race":
                    Game.GoToLevel(Game.PreviousLevel);
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
