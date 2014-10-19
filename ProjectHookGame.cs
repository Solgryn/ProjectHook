#region Using Statements

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using GrappleRace.GameFrameWork;
using LevelReader.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectHook.GameFrameWork;
using ProjectHook.Menu;

#endregion

namespace ProjectHook
{
    /// <summary>
    ///     This is the main type for your game
    /// </summary>
    public class ProjectHookGame : GameHost
    {
        private bool _canPressKey = true; //Make sure key press doesn't loop
        
        private bool _drawHitboxes;
        private SpriteFont _font;

        private TextObject FirstPlace;

        private FontRenderer _raceTimerText;
        private FontRenderer _firstPlaceText;

        private GraphicsDeviceManager _graphics;
        private TiledMap _level;
        private MapObject _mapObject;
        private Player _player1;
        private Player _player2;
        private SpriteBatch _spriteBatch;
        private Texture2D _tiles;
        private bool _canPressEnter;

        private bool initialized;


        public ProjectHookGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";


            //Change screen size

            _graphics.PreferredBackBufferWidth = Camera.Width;
            _graphics.PreferredBackBufferHeight = Camera.Height;
        }

        /// <summary>
        ///     Allows the game to perform any initialization it needs to before starting to run.
        ///     This is where it can query for any required services and load any non-graphic
        ///     related content.  Calling base.Initialize will enumerate through any components
        ///     and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            _font = Content.Load<SpriteFont>("MonoLog");

            //Setup menus
            Globals.TitleScreen = new TitleScreen(this, _font, new Vector2(0, 0));
            Globals.StageSelect = new StageSelect(this, _font, new Vector2(0, 0));
            Globals.ResultScreen = new Results(this, _font, new Vector2(0, 0));

            CurrentLevel = Globals.Levels.None;

            //Set to full screen
            //_graphics.IsFullScreen = true;
            //_graphics.ApplyChanges();

            _font = Content.Load<SpriteFont>("MonoLog");
            Fonts.Add("thefont", _font);

            base.Initialize();
        }

        /// <summary>
        ///     LoadContent will be called once per game and is the place to load
        ///     all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            //Only load players if in a level
            if (CurrentLevel != Globals.Levels.None)
            {
                var fishTx = Content.Load<Texture2D>("fish");
                _player1 = new Player(this, new Vector2(175, 150), fishTx, PlayerIndex.One);
                _player2 = new Player(this, new Vector2(210, 150), fishTx, PlayerIndex.Two);
                Collections.Players.Add(_player1);
                Collections.Players.Add(_player2);
            }

            //Reset camera position
            Camera.Position = Vector2.Zero;

            //Set start levels and menus
            if (!initialized)
            {
                CurrentLevel = 0;
                CurrentMenu = Globals.TitleScreen;
                initialized = true;
            }

            //Show menu else show level
            if (CurrentMenu != null)
            {
                CurrentMenu.ShowMenu();
            }
            else
            {
                GameObjects.Add(_player1);
                GameObjects.Add(_player2); 
                _tiles = Content.Load<Texture2D>("tiles");
                _level = new TiledMap("Levels/" + CurrentLevel.ToDescription() + ".tmx"); //Which level to load
                _mapObject = new MapObject(this, new Vector2(-64, 0), _tiles, _level); //Adds the level tiles to the global collection

                //Make bitmap texts
                _raceTimerText = Content.NewFont("bitmapfont", new Vector2(Camera.Width/2f, 8), FontRenderer.FontDisplays.Center);
                Collections.Fonts.Add(_raceTimerText);
                _firstPlaceText = Content.NewFont("bitmapfont", new Vector2(0, 48), FontRenderer.FontDisplays.Center);
                Collections.Fonts.Add(_firstPlaceText);
            }
        }

        /// <summary>
        ///     UnloadContent will be called once per game and is the place to unload
        ///     all content.
        /// </summary>
        protected override void UnloadContent()
        {
            GameObjects.Clear();
            Collections.Players.Clear();
            Collections.Tiles.Clear();
            Collections.Fonts.Clear();
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Only change the camera if in a level
            if (CurrentLevel != Globals.Levels.None)
            {
                //Set camera position
                Camera.Position.X =
                    Camera.Position.X.SmoothTowards(Collections.Players.Average(player => player.PositionX) - 250, 0.1f);
                //Set x position to average of all players
                Camera.Position.Y = 0;

                Camera.Position.X = Math.Max(Camera.Position.X, 0); //Limit camera
            }
            
            //Toggle drawing of hitboxes
            if (Keyboard.GetState().IsKeyDown(Keys.F1) && _canPressKey)
            {
                _drawHitboxes = !_drawHitboxes;
                _canPressKey = false;
            }

            if (Keyboard.GetState().IsKeyUp(Keys.F1))
                _canPressKey = true;

            //TODO add controller buttons
            //Menu controls
            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && _canPressEnter && CurrentMenu != null)
            {
                _canPressEnter = false;
                CurrentMenu.OpenSelection();
            }

                //Make the enter key not repeat
                if (Keyboard.GetState().IsKeyUp(Keys.Enter) && !_canPressEnter)
                    _canPressEnter = true;

            //Update menu if there is one open
            if (CurrentMenu != null)
                CurrentMenu.Update(gameTime);

            //Change levels
            if (Keyboard.GetState().IsKeyDown(Keys.F5))
                GoToMenu(Globals.TitleScreen);

            //Switch levels (admin control)
            if (Keyboard.GetState().IsKeyDown(Keys.F2))
                    GoToLevel(Globals.Levels.Level1);

            if (Keyboard.GetState().IsKeyDown(Keys.F3))
                GoToLevel(Globals.Levels.Level2);

            if (Keyboard.GetState().IsKeyDown(Keys.F4))
                GoToMenu(Globals.ResultScreen);

            //Quit game
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
        
            for (var i = 0; i < GameObjects.Count; i++)
            {
                GameObjects[i].Update(gameTime);
            }

            //If in a level and racing, display the timer and who's in the lead
            if (CurrentLevel != null && CurrentRace != null && CurrentRace.IsStarted)
            {
                CurrentRace.CalcFirstPlace();
                CurrentRace.Update(gameTime);
                _firstPlaceText.Text = "Player " + CurrentRace.FirstPlace + " is in the lead!\n";
                _firstPlaceText.Position.X = Camera.Width/2f;
                _raceTimerText.Text = CurrentRace.Timer.GetAsText();
            }

            base.Update(gameTime);
        }
        
        /// <summary>
        ///     This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            //Draw tiles
            foreach (var tile in Collections.Tiles)
            {
                tile.Draw(gameTime, _spriteBatch);
            }
            //Draw game objects
            foreach (var gameObjectBase in GameObjects)
            {
                var gameObject = (SpriteObject) gameObjectBase;
                gameObject.Draw(gameTime, _spriteBatch);
            }
            //Draw bitmap fonts
            foreach (var fontRenderer in Collections.Fonts)
            {
                fontRenderer.DrawText(_spriteBatch);
            }
            
            //Draw hitboxes
            if (_drawHitboxes)
            {
                foreach (var gameObjectBase in GameObjects)
                {
                    var gameObject = (SpriteObject) gameObjectBase;
                    _spriteBatch.Draw(
                        generateTexture2D(gameObject.BoundingBox.Width, gameObject.BoundingBox.Height, Color.Red),
                        new Vector2(gameObject.BoundingBox.Location.X + (-Camera.Position.X),
                            gameObject.BoundingBox.Location.Y + Camera.Position.Y)*Camera.Scale);
                }
            }

            _spriteBatch.End();

            //Draw menu, if there is one
            if(CurrentMenu != null)
                CurrentMenu.Draw(gameTime, _spriteBatch);

            base.Draw(gameTime);
        }

        private Texture2D generateTexture2D(int width, int height, Color textureColor)
        {
            var rectangleTexture = new Texture2D(GraphicsDevice, width, height, false, SurfaceFormat.Color);
            var color = new Color[width*height];
            for (var i = 0; i < color.Length; i++)
            {
                color[i] = textureColor;
            }
            rectangleTexture.SetData(color); //set the color data on the texture
            return rectangleTexture; //return the texture
        }

        public override void GoToLevel(Globals.Levels level)
        {
            CloseCurrentMenu();
            UnloadContent();
            CurrentLevel = level;
            LoadContent();

            //Start new race when a new level is started
            CurrentRace = new Race();
            CurrentRace.StartRace();
        }

        public override void GoToMenu(IMenu menu)
        {
            CurrentLevel = Globals.Levels.None;
            CloseCurrentMenu();
            CurrentMenu = menu;
            UnloadContent();
            LoadContent();
        }

        public void CloseCurrentMenu()
        {
            if (CurrentMenu == null) return;
            CurrentMenu.CloseMenu();
            CurrentMenu = null;
        }

        
    }
}