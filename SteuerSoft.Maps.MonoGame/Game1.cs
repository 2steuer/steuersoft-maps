using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SteuerSoft.Maps.Caching;
using SteuerSoft.Maps.Controls.MonoGame;
using SteuerSoft.Maps.Controls.MonoGame.MapExtensions;
using SteuerSoft.Maps.Controls.MonoGame.Material;
using SteuerSoft.Maps.Controls.MonoGame.ValueTypes;
using SteuerSoft.Maps.Core.Material;
using SteuerSoft.Maps.Core.Material.Elements.Layers;
using SteuerSoft.Maps.Core.Material.Elements.Path;
using SteuerSoft.Maps.MonoGame.OsmExtensions;
using SteuerSoft.Osm.Loading;
using SteuerSoft.Osm.Material;
using SteuerSoft.Osm.PathFinding.Algorithms;
using SteuerSoft.Osm.StreetNetwork;
using SteuerSoft.Osm.StreetNetwork.Material;
using SteuerSoft.Osm.StreetNetwork.Singleton;

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
        private MapLayer _routeLayer;
        private MapPath _currentPath;

        private int _clickIndex = 0;

        private Waypoint _startPoint = null;
        private Waypoint _endPoint = null;

        private OsmLoader _osm = null;
        private OsmStreetSystem _streets = null;


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
            Defaults.Load();
            _streets = OsmStreetSystem.LoadSystem("K:\\OsmData\\schleswig-holstein-latest.sn");

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            _map = new MapControl(spriteBatch, GraphicsDevice, GraphicsDevice.Viewport.Bounds.ToMapRectangle());
            _map.Zoom = 15;
            _map.Position = new MapPointLatLon(53.8265376, 10.4917827);
            _map.ZoomMode = ZoomingType.Mouse;

            _routeLayer = _map.AddLayer();

            _map.OnRightClick += _map_OnRightClick;
        }

        private void _map_OnRightClick(object sender, MapPointLatLon pos)
        {
            Waypoint click = new Waypoint(new OsmNode(1, pos.Lat, pos.Lon));

            Waypoint nearest = _streets.Waypoints.Values.OrderBy(w => w.DistanceTo(click)).First();

            if (_clickIndex == 0)
            {
                if (_currentPath != null)
                {

                    _map.RemovePath(_currentPath);
                    _currentPath = null;
                }

                _startPoint = nearest;
                _endPoint = null;

                _clickIndex = 1;
            }
            else if (_clickIndex == 1)
            {
                _endPoint = nearest;

                var p = _streets.FindPath(_startPoint, _endPoint);
                if (p != null)
                {
                    _currentPath = _map.AddPath(_routeLayer, p.Waypoints.Select(wp => new MapPointLatLon(wp.Lat, wp.Lon)), Color.Red);
                }

                _clickIndex = 0;
            }
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

            InputArgs args = new InputArgs();
            args.MouseState = Mouse.GetState();
            args.KeyboardState = Keyboard.GetState();

            _map.Update(gameTime, args);

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
