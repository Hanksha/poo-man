using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using PacmanGame.GameStates;
using System.Diagnostics;
using System.Xml;

namespace PacmanGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;
        public Matrix modelView;

        // Constants
        public const int TILE_SIZE = 32;
        public const int WIDTH = 1280;
        public const int HEIGHT = 720;

        // Texture Atlas 
        Texture2D texAtlas;
        public static SpriteFont fontBig;
        public static SpriteFont fontSmall;

        SoundBank soundBank;

        GameStateManager gsm;

        InputProcessor input;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;
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
            input = new InputProcessor();
            modelView = new Matrix(
                new Vector4(1, 0, 0, 0),
                new Vector4(0, 1, 0, 0),
                new Vector4(0, 0, 1, 0),
                new Vector4(0, 0, 0, 1)
                );

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load fonts
            fontBig = this.Content.Load<SpriteFont>("MithrilFontBig");
            fontSmall = this.Content.Load<SpriteFont>("MithrilFontSmall");

            soundBank = new SoundBank();

            soundBank.addSfx("intro_theme", Content.Load<SoundEffect>("intro_theme"));
            soundBank.addSfx("dog_woof", Content.Load<SoundEffect>("dog_woof"));
            soundBank.addSfx("dog_whine", Content.Load<SoundEffect>("dog_whine"));
            soundBank.addSfx("dog_grr", Content.Load<SoundEffect>("dog_grr"));
            soundBank.addSfx("engine_loop", Content.Load<SoundEffect>("engine_loop"));
            soundBank.addSfx("eat1", Content.Load<SoundEffect>("eat1"));
            soundBank.addSfx("eat2", Content.Load<SoundEffect>("eat2"));
            soundBank.addSfx("eat3", Content.Load<SoundEffect>("eat3"));
            soundBank.addSfx("explo", Content.Load<SoundEffect>("explo"));
            soundBank.addSfx("bonus", Content.Load<SoundEffect>("bonus"));
            soundBank.addSfx("energizer", Content.Load<SoundEffect>("energizer"));
            soundBank.addSfx("blip", Content.Load<SoundEffect>("blip"));
            soundBank.addSfx("line_win", Content.Load<SoundEffect>("line_win"));
            soundBank.addSfx("line_lose", Content.Load<SoundEffect>("line_lose"));

            soundBank.setTheme(Content.Load<Song>("theme"));

            // Load texture atlas
            texAtlas = this.Content.Load<Texture2D>("pacman-texatlas.png");
            
            gsm = new GameStateManager(this, input, soundBank, texAtlas);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
            Content.Unload();
            spriteBatch.Dispose();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            input.update();

            if (input.isKeyPressed(Keys.F))
                graphics.ToggleFullScreen();

            if (input.isKeyPressed(Keys.M))
                soundBank.Enabled = !soundBank.Enabled;

            gsm.update(gameTime.ElapsedGameTime.TotalSeconds);

            soundBank.update();

            base.Update(gameTime);

        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
          
            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, modelView);

            gsm.render(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
