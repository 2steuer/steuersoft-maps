using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SteuerSoft.Maps.Caching;
using SteuerSoft.Maps.Core.Material;
using SteuerSoft.Maps.MonoGame.MapExtensions;
using SteuerSoft.Maps.Providers;

namespace SteuerSoft.Maps.MonoGame.Map
{
   /// <summary>
   /// A MapControl that shall be drawn within a MonoGame game window.
   /// </summary>
   class MapControl
   {
      /// <summary>
      /// The SpriteBatch that shall be used to draw the map on.
      /// </summary>
      private SpriteBatch _sBatch;

      /// <summary>
      /// The GraphicsDevice on which we will draw. This is mainly used to create textures.
      /// </summary>
      private GraphicsDevice _device;

      /// <summary>
      /// The TiledMapProvider behind this control. Manages all logic things.
      /// </summary>
      private TiledMapProvider _map;

      /// <summary>
      /// Currently loaded tiles.
      /// </summary>
      Dictionary<TileDescriptor, Texture2D> _tiles = new Dictionary<TileDescriptor, Texture2D>();

      /// <summary>
      /// The loaded tiles in order of loading. To remove tiles that were not used for a while.
      /// </summary>
      private List<TileDescriptor> _loadedTiles = new List<TileDescriptor>();

      /// <summary>
      /// For Lines and Rectangles drawing. Just a pixel.
      /// </summary>
      Texture2D _pixel;

      /// <summary>
      /// If true, a border is drawn around each Tile. For Debugging purposes.
      /// </summary>
      public bool DrawTileBorders { get; set; } = false;

      /// <summary>
      /// If true, a cross is drawn in the middle of the map to indicate the middle point.
      /// The cross will be at the "Position" of the map.
      /// </summary>
      public bool DrawMiddleCross { get; set; } = true;

      /// <summary>
      /// The color of the cross in the middle.
      /// </summary>
      public Color MiddleCrossColor { get; set; } = Color.Green;

      /// <summary>
      /// The thickness of the cross in the middle.
      /// </summary>
      public int MiddleCrossThickness { get; set; } = 2;

      /// <summary>
      /// The maximum number of tiles that shall be held in
      /// the Texture memory. Higher means fast loading but more memory consumption.
      /// </summary>
      public int MaxLoadedTiles { get; set; } = 128;

      /// <summary>
      /// Represents the last mouse state.
      /// Is overwritten in every Update() call with the current mouse state.
      /// </summary>
      MouseState _oldMouseState = new MouseState();

      /// <summary>
      /// Indicates wether the mouse is currently dragging the map.
      /// </summary>
      private bool _dragging = false;

      /// <summary>
      /// Initialises a new instance of the MapControl class.
      /// Sets OpenStreetMap_Original and Disc Caching as the default Map and caching providers.
      /// Sets MinZoom of the default Map Proider as the Zoom and (0,0) as the position.
      /// </summary>
      /// <param name="batch">The SpriteBatch to use for drawing.</param>
      /// <param name="device">The graphicsDevice to draw the map on.</param>
      /// <param name="bounds">The bounds rectangle on the viewport at which the map shall be drawn.</param>
      public MapControl(SpriteBatch batch, GraphicsDevice device, MapRectangle bounds)
      {
         _sBatch = batch;
         _device = device;
         
         _map = new TiledMapProvider(OsmProvider.GetInstance(), new DiscTileCaching("cache"));

         _map.Zoom = _map.Provider.MinZoom;
         _map.Position = new MapPointLatLon(0,0);

         _map.ViewBounds = bounds;

         // Somewhere in your LoadContent() method:
         // Set the pixel texture.
         _pixel = new Texture2D(_device, 1, 1, false, SurfaceFormat.Color);
         _pixel.SetData(new[] {Color.White});
      }

      /// <summary>
      /// Shall be called when the map is to be drawn.
      /// Draws the map within the given boundaries.
      /// </summary>
      /// <param name="time">Elapsed time since program start.</param>
      public void Draw(GameTime time)
      {
         _sBatch.Begin();

         foreach (var tile in _map.GetDrawTiles())
         {
            DrawTile(tile.Tile, tile.SourceRectangle.ToRectangle(), tile.DestinationRectangle.ToRectangle());
         }
         
         Rectangle crossRect = new Rectangle(_map.ViewBounds.X + (_map.ViewBounds.Width / 2 - 25), _map.ViewBounds.Y + (_map.ViewBounds.Height / 2 - 25), 50, 50);

         if (DrawMiddleCross)
         {
            DrawCross(crossRect, MiddleCrossThickness, MiddleCrossColor);
         }
         

         _sBatch.End();
      }

      /// <summary>
      /// Draws a tile on the ViewPort. Takes the data returned by the map provider
      /// regarding Tile description, source and destination rectangle and simply draws it on the viewport.
      /// </summary>
      /// <param name="desc">The tile that shall be drawn.</param>
      /// <param name="srcRect">The rectangle of the tile that will be drawn</param>
      /// <param name="dstRect">The position and size where the tile shall be drawn on the viewport.</param>
      private void DrawTile(TileDescriptor desc, Rectangle srcRect, Rectangle dstRect)
      {
         Texture2D texture = GetTileTexture(desc);
         if (texture != null)
         {
            _sBatch.Draw(texture, dstRect, srcRect, Color.White);

            if (DrawTileBorders)
            {
               DrawBorder(dstRect, 2, Color.Red);
            }
         }
      }

      /// <summary>
      /// Returns a texture2d of the given tile.
      /// Loads the Tile Stream from the map provider and turns it into a texture.
      /// If the texture is not available, it returns null.
      /// Also takes care of haven onnly MacTileCount tiles in the memory, i.e. removing textures that
      /// were not used for a long time.
      /// </summary>
      /// <param name="desc">The tile descriptor of the tile that shall be loaded.</param>
      /// <returns>A Texture2D of the given tile or null.</returns>
      private Texture2D GetTileTexture(TileDescriptor desc)
      {
         if (!_tiles.ContainsKey(desc))
         {
            Stream s = _map.GetTile(desc.Zoom, desc.Tile);

            if (s != null)
            {
               // s will be disposed after the tile has been loaded.
               using (s)
               {
                  _tiles.Add(desc, Texture2D.FromStream(_device, s));
               }

               // Handle the max tiles in memory
               // Add the current tile to the last place in the list

               _loadedTiles.Remove(desc);
               _loadedTiles.Add(desc);


               // Now remove as many tiles from the front of the list until
               // we have only MacLoadedTiles loaded.
               while (_loadedTiles.Count > MaxLoadedTiles)
               {
                  var t = _loadedTiles[0];
                  _loadedTiles.RemoveAt(0);

                  var text = _tiles[t];
                  text.Dispose();
                  _tiles.Remove(t);
               }

               return _tiles[desc];
            }
            else
            {
               return null;
            }
         }
         else
         {
            return _tiles[desc];
         }
      }

      /// <summary>
      /// Will draw a border (hollow rectangle) of the given 'thicknessOfBorder' (in pixels)
      /// of the specified color.
      ///
      /// By Sean Colombo, from http://bluelinegamestudios.com/blog
      /// </summary>
      /// <param name="rectangleToDraw">The rectangle around which the border shall be drawn.</param>
      /// <param name="thicknessOfBorder">The thickness of the border that is drawn.</param>
      private void DrawBorder(Rectangle rectangleToDraw, int thicknessOfBorder, Color borderColor)
      {
         // Draw top line
         _sBatch.Draw(_pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, rectangleToDraw.Width, thicknessOfBorder), borderColor);

         // Draw left line
         _sBatch.Draw(_pixel, new Rectangle(rectangleToDraw.X, rectangleToDraw.Y, thicknessOfBorder, rectangleToDraw.Height), borderColor);

         // Draw right line
         _sBatch.Draw(_pixel, new Rectangle((rectangleToDraw.X + rectangleToDraw.Width - thicknessOfBorder),
                                            rectangleToDraw.Y,
                                            thicknessOfBorder,
                                            rectangleToDraw.Height), borderColor);
         // Draw bottom line
         _sBatch.Draw(_pixel, new Rectangle(rectangleToDraw.X,
                                         rectangleToDraw.Y + rectangleToDraw.Height - thicknessOfBorder,
                                         rectangleToDraw.Width,
                                         thicknessOfBorder), borderColor);
      }

      /// <summary>
      /// Draws a cross within the given rectangle.
      /// The cross middle point will be the middle of the passed rectangle.
      /// </summary>
      /// <param name="rect">The rectangle to draw the cross in.</param>
      /// <param name="thickness">The thickness of the cross lines.</param>
      /// <param name="crossColor">The color of the cross lines.</param>
      private void DrawCross(Rectangle rect, int thickness, Color crossColor)
      {
         // 1. vertical line
         Rectangle vert = new Rectangle(rect.X + (rect.Width / 2 - thickness / 2), rect.Y, thickness, rect.Height);

         // 2. horizontal line
         Rectangle hori = new Rectangle(rect.X, rect.Y +  (rect.Height / 2 - thickness / 2), rect.Width, thickness);

         _sBatch.Draw(_pixel, vert, crossColor);
         _sBatch.Draw(_pixel, hori, crossColor);
      }

      /// <summary>
      /// Handles the logic of Dragging/Dropping/Zooming the map.
      /// This is basically said the logic handler of the control.
      /// It is called quite a lot of times, the delta of two calls may be calculated out of the passed GameTime parameter.
      /// </summary>
      /// <param name="time">The time passed since the game started.</param>
      public void Update(GameTime time)
      {
         MouseState currentState = Mouse.GetState();

         if (_dragging)
         {
            MapPoint oldPoint = new MapPoint() {X = _oldMouseState.X, Y = _oldMouseState.Y};
            MapPoint newPoint = new MapPoint() {X = currentState.X, Y = currentState.Y};

            MapPoint offset = (oldPoint - newPoint) / _map.CoordinateScale;

            MapPoint centerMapPoint = _map.LatLonToMapPoint(_map.Position);
            MapPoint newCenterMapPoint = centerMapPoint + offset;

            // real a mod b:
            // a mod b = (a % b + b) % b
            // https://de.wikipedia.org/wiki/Division_mit_Rest#Modulo
            // We do this to allow scrollen over the maps borders.
            newCenterMapPoint.X = ((newCenterMapPoint.X%_map.MapCoordinatesWidth) + _map.MapCoordinatesWidth) % _map.MapCoordinatesWidth;
            newCenterMapPoint.Y = ((newCenterMapPoint.Y%_map.MapCoordinatesHeight) + _map.MapCoordinatesHeight) % _map.MapCoordinatesHeight;
            

            MapPointLatLon newCenterGeoPoint = _map.MapPointToLatLon(newCenterMapPoint);

            _map.Position = newCenterGeoPoint;
         }

         int wheel = _oldMouseState.ScrollWheelValue - currentState.ScrollWheelValue;

         if (wheel != 0)
         {
            Console.WriteLine();
         }

         if (wheel < 0)
         {
            if (_map.Zoom < _map.MaxZoom)
            {
               _map.Zoom++;
            }
            
         }
         else if(wheel > 0)
         {
            if (_map.Zoom > _map.MinZoom)
            {
               _map.Zoom--;
            }
         }

         if ((_oldMouseState.LeftButton == ButtonState.Pressed) && currentState.LeftButton == ButtonState.Released)
         {
            // Mouse up
            _dragging = false;
         }
         else if ((_oldMouseState.LeftButton == ButtonState.Released) && currentState.LeftButton == ButtonState.Pressed)
         {
            // Mouse down
            _dragging = true;
         }

         _oldMouseState = currentState;
      }
   }
}
