#region Using Statements

using System;
using System.ComponentModel;
using System.Diagnostics;
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
        private PauseMenu _pauseMenu;

        private GraphicsDeviceManager _graphics;
        private TitleScreen _titleScreen;
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
            var fishTx = Content.Load<Texture2D>("fish");
            _font = Content.Load<SpriteFont>("MonoLog");
            _player1 = new Player(this, new Vector2(175, 150), fishTx, PlayerIndex.One);
            _player2 = new Player(this, new Vector2(210, 150), fishTx, PlayerIndex.Two);
            _titleScreen = new TitleScreen(this, _font, new Vector2(0, 0));
            _pauseMenu = new PauseMenu(this, _font, new Vector2(0, 0));

            Camera.Position = Vector2.Zero;

            //Set start levels and menus
            if (!initialized)
            {
                CurrentLevel = Globals.Levels.Level1;
                CurrentMenu = _titleScreen;
                initialized = true;
            }

            Collections.Players.Add(_player1);
            Collections.Players.Add(_player2);

            //Tiles
            if (CurrentMenu != null)
            {
                CurrentMenu.ShowMenu();
            }
            else
            {
                GameObjects.Add(_player1);
                GameObjects.Add(_player2);
                //if (_titleScreen._MenuState == TitleScreen.MenuState.Multi)   
                _tiles = Content.Load<Texture2D>("tiles");
                _level = new TiledMap("Levels/" + CurrentLevel.ToDescription() + ".tmx");
                _mapObject = new MapObject(this, new Vector2(0, 0), _tiles, _level);

                GameObjects.Add(_mapObject);
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
            Camera.Position.Y = 175;

            //Camera.Zoom = Camera.Zoom.SmoothTowards(200, 0.01f);

            Camera.Position.X = Math.Max(Camera.Position.X, 32); //Limit camera

            //Toggle drawing of hitboxes
            if (Keyboard.GetState().IsKeyDown(Keys.F1) && _canPressKey)
            {
                _drawHitboxes = !_drawHitboxes;
                _canPressKey = false;
            }

            if (Keyboard.GetState().IsKeyUp(Keys.F1))
                _canPressKey = true;

            if (Keyboard.GetState().IsKeyDown(Keys.Enter) && _canPressEnter)
            {
                _canPressEnter = false;
                CurrentMenu.OpenSelection();
            }

            if (Keyboard.GetState().IsKeyUp(Keys.Enter) && !_canPressEnter)
            {
                _canPressEnter = true;
            }

            //Update menu if there is one open
            if (CurrentMenu != null)
            {
                CurrentMenu.Update(gameTime);
            }

            //Change levels
            if (Keyboard.GetState().IsKeyDown(Keys.F5))
                GoToMenu(_titleScreen);

            //Switch levels (admin control)
            if (Keyboard.GetState().IsKeyDown(Keys.F2))
                    GoToLevel(Globals.Levels.Level1);

            if (Keyboard.GetState().IsKeyDown(Keys.F3))
                GoToLevel(Globals.Levels.Level2);

            //Quit game
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            
        
            if(!_pauseMenu.IsMenuOpen)
            { 
                 for (int index = 0; index < GameObjects.Count; index++)
                {
                    GameObjects[index].Update(gameTime);
                }
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

            foreach (Tile tile in Collections.Tiles)
            {
                tile.Draw(gameTime, _spriteBatch);
            }
            foreach (SpriteObject gameObject in GameObjects)
            {
                gameObject.Draw(gameTime, _spriteBatch);
            }


            //Draw hitboxes
            if (_drawHitboxes)
            {
                foreach (SpriteObject gameObject in GameObjects)
                {
                    _spriteBatch.Draw(
                        generateTexture2D(gameObject.BoundingBox.Width, gameObject.BoundingBox.Height, Color.Red),
                        new Vector2(gameObject.BoundingBox.Location.X + (-Camera.Position.X),
                            gameObject.BoundingBox.Location.Y + Camera.Position.Y)*Camera.Scale);
                }
            }

            _spriteBatch.End();
            if (CurrentLevel == Globals.Levels.TitleScreen) _titleScreen.Draw(gameTime, _spriteBatch);
            if (CurrentLevel == Globals.Levels.PauseMenu) _pauseMenu.Draw(gameTime, _spriteBatch);
            base.Draw(gameTime);
        }

        private Texture2D generateTexture2D(int width, int height, Color textureColor)
        {
            var rectangleTexture = new Texture2D(GraphicsDevice, width, height, false, SurfaceFormat.Color);
            var color = new Color[width*height];
            for (int i = 0; i < color.Length; i++)
            {
                color[i] = textureColor;
            }
            rectangleTexture.SetData(color); //set the color data on the texture
            return rectangleTexture; //return the texture
        }

        public override void GoToLevel(Globals.Levels level)
        {
            CurrentMenu = null;
            UnloadContent();
            CurrentLevel = level;
            LoadContent();
        }

        public void GoToMenu(IMenu menu)
        {
            CurrentMenu = menu;
            LoadContent();
        }
    }
}