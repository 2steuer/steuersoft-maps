using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Framework.WpfInterop;
using MonoGame.Framework.WpfInterop.Input;
using SteuerSoft.Maps.Controls.MonoGame;
using SteuerSoft.Maps.Controls.MonoGame.MapExtensions;
using SteuerSoft.Maps.Controls.MonoGame.ValueTypes;
using SteuerSoft.Maps.Core.Material;

namespace SteuerSoft.Maps.Wpf
{
    public delegate void InitializedDelegate(object sender);

    class MapGame : WpfGame
    {
        public event InitializedDelegate DisplayInitialized;

        private IGraphicsDeviceService _deviceSrv;
        private SpriteBatch _sbatch;

        private WpfMouse _mouse;
        private WpfKeyboard _keyboard;

        private MapControl _map;

        internal MapControl Map => _map;

        public MapGame()
        {

        }

        protected override void Initialize()
        {

            _deviceSrv = new WpfGraphicsDeviceService(this);
            _mouse = new WpfMouse(this);
            _keyboard = new WpfKeyboard(this);

            _sbatch = new SpriteBatch(GraphicsDevice);
            _map = new MapControl(_sbatch, GraphicsDevice, GraphicsDevice.Viewport.Bounds.ToMapRectangle());

            base.Initialize();

            DisplayInitialized?.Invoke(this);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            _map.ViewBounds = GraphicsDevice.Viewport.Bounds.ToMapRectangle();

            _map.Update(gameTime, _mouse.GetState(), _keyboard.GetState());

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
            _map.Draw(gameTime);
            base.Draw(gameTime);
        }


    }
}
