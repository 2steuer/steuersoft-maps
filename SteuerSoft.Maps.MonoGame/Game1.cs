using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SteuerSoft.Maps.Caching;
using SteuerSoft.Maps.Core.Material;
using SteuerSoft.Maps.MonoGame.Map;
using SteuerSoft.Maps.MonoGame.MapExtensions;
using SteuerSoft.Maps.Providers;

namespace SteuerSoft.Maps.MonoGame
{
   /// <summary>
   /// This is the main type for your game.
   /// </summary>
   public class Game1 : Game
   {
      GraphicsDeviceManager graphics;
      SpriteBatch spriteBatch;

      private MapControl _map;

      public Game1()
      {
         graphics = new GraphicsDeviceManager(this);

         graphics.PreferredBackBufferWidth = 1024;  // set this value to the desired width of your window
         graphics.PreferredBackBufferHeight = 768;   // set this value to the desired height of your window
         graphics.ApplyChanges();


         Content.RootDirectory = "Content";

         IsMouseVisible = true;
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
         spriteBatch = new SpriteBatch(GraphicsDevice);

         // TODO: use this.Content to load your game content here

         _map = new MapControl(spriteBatch, GraphicsDevice, GraphicsDevice.Viewport.Bounds.ToMapRectangle());
        
      }

      /// <summary>
      /// UnloadContent will be called once per game and is the place to unload
      /// game-specific content.
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

         if (!IsActive)
            return;

         _map.Update(gameTime);

         base.Update(gameTime);
      }

      /// <summary>
      /// This is called when the game should draw itself.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values.</param>
      protected override void Draw(GameTime gameTime)
      {
         GraphicsDevice.Clear(Color.CornflowerBlue);

         _map.Draw(gameTime);

         base.Draw(gameTime);
      }
   }
}
