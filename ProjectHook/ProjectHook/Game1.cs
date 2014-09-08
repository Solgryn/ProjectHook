﻿#region Using Statements
using GrappleRace.GameFrameWork;
using LevelReader.GameFrameWork;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
#endregion

namespace ProjectHook
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : GameHost
    {
        enum Levels
        {
            Level1
        }

        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;

        private Player _player1;
        private Player _player2;
        private Cloud _cloud;

        private Texture2D _tiles;
        private TiledMap _level;
        private bool _levelLoaded = false;
        private string _levelInfo;
        private SpriteFont _font;
        private MapObject _mapObject;
        
        public Game1()
            : base()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            var fishTx = Content.Load<Texture2D>("fish");
            _player1 = new Player(this, new Vector2(100, 100), fishTx, PlayerIndex.One);
            _player2 = new Player(this, new Vector2(200, 100), fishTx, PlayerIndex.Two);
            var cloudTx = Content.Load<Texture2D>("cloud");
            _cloud = new Cloud(this, new Vector2(0,150), cloudTx);

            GameObjects.Add(_cloud);
            GameObjects.Add(_player1);
            //GameObjects.Add(_player2);

            Collections.Players.Add(_player1);
            //Collections.Players.Add(_player2);

            //Tiles
            _tiles = Content.Load<Texture2D>("tiles");

            _font = Content.Load<SpriteFont>("MonoLog");

            _level = new TiledMap("Levels/Level1.tmx");

            _mapObject = new MapObject(this, new Vector2(0, 0), _tiles, _level, _font);
            //_mapObject.Scale = new Vector2(2,2);
            GameObjects.Add(_mapObject);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            for (var index = 0; index < GameObjects.Count; index++)
            {
                GameObjects[index].Update(gameTime);
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightSkyBlue);

            _spriteBatch.Begin();
            foreach (SpriteObject gameObject in GameObjects)
            {
                gameObject.Draw(gameTime, _spriteBatch);
            }
            foreach (var tile in Collections.Tiles)
            {
                tile.Draw(gameTime, _spriteBatch);
            }

            //Draw hitboxes
            foreach (SpriteObject gameObject in GameObjects)
            {
                _spriteBatch.Draw(generateTexture2D(gameObject.BoundingBox.Width, gameObject.BoundingBox.Height, Color.Red), new Vector2(gameObject.BoundingBox.Location.X, gameObject.BoundingBox.Location.Y));
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private Texture2D generateTexture2D(int width, int height, Color textureColor)
        {
            Texture2D rectangleTexture = new Texture2D(this.GraphicsDevice, width, height, false, SurfaceFormat.Color);
            Color[] color = new Color[width * height];
            for (int i = 0; i < color.Length; i++)
            {
                color[i] = textureColor;
            }
            rectangleTexture.SetData(color);//set the color data on the texture
            return rectangleTexture;//return the texture
        }
    }
}
