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

        public Race CurrentRace;
        private TextObject FirstPlace;

        private FontRenderer _fontRenderer;

        private Timer _timer;
        private TextObject _record;

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

            if (CurrentLevel != null)
            {
                var fishTx = Content.Load<Texture2D>("fish");
                _player1 = new Player(this, new Vector2(175, 150), fishTx, PlayerIndex.One);
                _player2 = new Player(this, new Vector2(210, 150), fishTx, PlayerIndex.Two);
            }

            Camera.Position = Vector2.Zero;

            //_timer = new Timer(this, _font, new Vector2(10, 10));

            //Set start levels and menus
            if (!initialized)
            {
                CurrentLevel = 0;
                CurrentMenu = Globals.TitleScreen;
                initialized = true;
            }

            Collections.Players.Add(_player1);
            Collections.Players.Add(_player2);

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
                _level = new TiledMap("Levels/" + CurrentLevel.ToDescription() + ".tmx");
                _mapObject = new MapObject(this, new Vector2(0, 0), _tiles, _level);

                var fontFilePath = Path.Combine(Content.RootDirectory, "bitmapfont.fnt");
                var fontFile = FontLoader.Load(fontFilePath);
                var fontTexture = Content.Load<Texture2D>("bitmapfont_0.png");

                _fontRenderer = new FontRenderer(fontFile, fontTexture);
                _fontRenderer.Text = "hej";
                Collections.Fonts.Add(_fontRenderer);

                FirstPlace = new TextObject(this, _font, new Vector2(10, 10), "Who's in first place?");
                GameObjects.Add(FirstPlace);
                //_record = new TextObject(this, _font, new Vector2(700, 10), _timer.ShowLevelRecord(CurrentLevel));
                //GameObjects.Add(_record);
                
                //GameObjects.Add(_timer);
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
        }

        /// <summary>
        ///     Allows the game to run logic such as updating the world,
        ///     checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //Set camera position
            Camera.Position.X =
                Camera.Position.X.SmoothTowards(Collections.Players.Average(player => player.PositionX) - 250, 0.1f);
            //Set x position to average of all players
            Camera.Position.Y = 0;

            Camera.Position.X = Math.Max(Camera.Position.X, 0); //Limit camera

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

            //Quit game
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
        
            for (var i = 0; i < GameObjects.Count; i++)
            {
                GameObjects[i].Update(gameTime);
            }

            //In a level, racing
            if (CurrentLevel != null && CurrentRace != null)
            {
                CurrentRace.Update(gameTime);
                FirstPlace.Text = "Player " + CurrentRace.GetFirstPlace() + " is in the lead!";
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

            foreach (var tile in Collections.Tiles)
            {
                tile.Draw(gameTime, _spriteBatch);
            }
            foreach (var gameObjectBase in GameObjects)
            {
                var gameObject = (SpriteObject) gameObjectBase;
                gameObject.Draw(gameTime, _spriteBatch);
            }

            foreach (var fontRenderer in Collections.Fonts)
            {
                fontRenderer.DrawText(_spriteBatch, 50, 50);
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

            //Start new race
            CurrentRace = new Race();
            CurrentRace.StartRace();
        }

        public override void GoToMenu(IMenu menu)
        {
            CloseCurrentMenu();
            CurrentMenu = menu;
            UnloadContent();
            LoadContent();
        }

        public override void FinishTime()
        {
            //_timer.FinishTime(CurrentLevel);
        }

        public void CloseCurrentMenu()
        {
            if (CurrentMenu == null) return;
            CurrentMenu.CloseMenu();
            CurrentMenu = null;
        }
    }
}