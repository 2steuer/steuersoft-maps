using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteuerSoft.Maps.Core.Interfaces;
using SteuerSoft.Maps.Core.Material;
using SteuerSoft.Maps.Core.Tools.Download;
using SteuerSoft.Maps.Core.Tools.Download.Material;

namespace SteuerSoft.Maps
{
   /// <summary>
   /// A Provider for the logic behind a tiled map view.
   /// 
   /// A word about the different Coordinate systems used within this class.
   /// <list type="table">
   /// <listheader>
   ///   <term>System</term>
   ///   <description>Description</description>
   /// </listheader>
   /// 
   /// <item>
   ///   <term>GeoCoordinates</term>
   ///   <description>The GeoCoordinates, also referred as GPS coordinates. Latitude and Longitude from -90/+90 and -180/180 in decimal format.</description>
   /// </item>
   /// 
   /// <item>
   ///   <term>ProjectionCoordinates</term>
   ///   <description>The GeoCoordinates, projected using the specified projection. Coordinate ranges may vary, for Mercator, they are
   ///  -180/180 for both dimensions.</description>
   /// </item>
   /// 
   /// <item>
   ///   <term>MapCoordinates</term>
   ///   <description>The projected coordinates, but scaled to pixels. The Width is TileCountX * TileWidth, the Height is TileCountY * TileHeight. 
   /// This coordinate system originates in the top-left corner, in contrast to the other coordinate systems in geo-coordiantes, which
   /// originate in the middle of the map.</description>
   /// </item>
   /// 
   /// <item>
   ///   <term>Tile Coordinates</term>
   ///   <description>The Coordinates of a tile. These are usually taken from the mercator projection in a specific way.
   /// For OpenStreetMap see an example unter https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#X_and_Y .
   /// Note, that a tile can only be identified by a zoom level and X/Y coordinates.</description>
   /// </item>
   /// 
   /// </list>
   /// </summary>
   public class TiledMapProvider
   {
      /// <summary>
      /// The current map provider used by the TiledMapProvider.
      /// </summary>
      private IMapProvider _mapProvider;

      /// <summary>
      /// The current caching provider.
      /// </summary>
      private ITileCachingProvider _cachingProvider;

      /// <summary>
      /// An Instance of the download service.
      /// </summary>
      private DownloadService _download;

      /// <summary>
      /// Counter for the server rotating.
      /// </summary>
      private int _serverRotator = 0;

      /// <summary>
      /// List of the enqueued tiles for downloading.
      /// </summary>
      HashSet<TileDescriptor> _enqueuedTiles = new HashSet<TileDescriptor>();

      /// <summary>
      /// The current Map Provider. This value can be changed in run-time.
      /// </summary>
      public IMapProvider Provider
      {
         get { return _mapProvider; }
         set
         {
            _download.MaxThreads = value.MaxDownloadThreads;
            _cachingProvider.SubCache = value.InternalName;
            _mapProvider = value;
            Zoom = Zoom; // To clip the current zoom value to the bounds of the new provider.
         }
      }

      #region
      /// <summary>
      /// The Width of the MapCoordinates system.
      /// </summary>
      public int MapCoordinatesWidth
      {
         get { return Provider.GetTileCount(MaxZoom) * Provider.TileSize; }
      }

      /// <summary>
      /// The height of the MapCoordinates system.
      /// </summary>
      public int MapCoordinatesHeight
      {
         get { return Provider.GetTileCount(Provider.MaxZoom) * Provider.TileSize; }
      }

      private int _zoom;

      /// <summary>
      /// The current zoom level of the map. Must be between MinZoom and MaxZoom.
      /// </summary>
      public int Zoom
      {
         get
         {
            return _zoom;
         }
         set
         {
            if (value < Provider.MinZoom)
            {
               _zoom = Provider.MinZoom;
            }
            else if (value > Provider.MaxZoom)
            {
               _zoom = Provider.MaxZoom;
            }
            else
            {
               _zoom = value;
            }
         }
      }

      /// <summary>
      /// This represents the factor 2^CurrentZoom / 2^MaxZoom of the current provider.
      /// This is needed to correctly transform MapCoordinates coordinates to ViewCoordinates.
      /// MapCoordinates are always the same, as per definition they are the maximum number of pixels when in 
      /// the highest zoom mode. This makes it possible to access every
      /// pixel on the whole world in the highest zoom level. To make it possible to easily transform
      /// coordinates etc. from current zoom level pixels (ViewCoordinates) to MapCoordinates pixels, this
      /// factor can be used. To get from MapCoordiantes to ViewCoordinates multiply by the factor.
      /// To Get from ViewCoordinates to MapCoordiantes, divide by the factor. 
      /// </summary>
      public double CoordinateScale
      {
         get { return (double)Provider.GetTileCount(Zoom) / Provider.GetTileCount(Provider.MaxZoom); }
      }

      /// <summary>
      /// The viewport the tiles are rendered to.
      /// This might as well be a Full screen beginning at 0,0 or 
      /// just a part of it, beginning elsewhere. 0,0 is always assumed the top-left corner.
      /// Tiles and Rectangles returned by TBD are sticked to this viewport.
      /// </summary>
      public MapRectangle ViewBounds { get; set; }

      /// <summary>
      /// The current position of the center of the map in GeoCoordinates.
      /// </summary>
      public MapPointLatLon Position { get; set; }
      #endregion

      /// <summary>
      /// Minimum zoom allowed by the current provider.
      /// </summary>
      public int MaxZoom => Provider.MaxZoom;

      /// <summary>
      /// Maximum zoom allowed by the current provider.
      /// </summary>
      public int MinZoom => Provider.MinZoom;

      /// <summary>
      /// Creates a new TiledMapProvider using the given Map Provider and the given Cache provider.
      /// </summary>
      /// <param name="provider">The Map Provider to initially use.</param>
      /// <param name="caching">The Cachine provider to use initially.</param>
      public TiledMapProvider(IMapProvider provider, ITileCachingProvider caching)
      {
         
         _cachingProvider = caching;
         _download = new DownloadService();

         Provider = provider;
      }

      /// <summary>
      /// Gets a given tile at a specific zoom level.
      /// Checks the current caching provider for the file. If the file is present within
      /// the caching provider, a stream to it's contents is returned. This stream should be disposed as soon as the
      /// data from it is fully used.
      /// 
      /// If the tile is not present within the tile cache, it is checked if the tile is currently being downloaded.
      ///  If not so, a download job to download the tile is enqueued.
      /// 
      /// In both cases - tile being downloaded or not - null is returned.
      /// </summary>
      /// <param name="zoom">The zoom level to get the tile from.</param>
      /// <param name="tile">The tile X/Y coordinates.</param>
      /// <returns>A stream to the tiles contents or null.</returns>
      public Stream GetTile(int zoom, MapVector tile)
      {
         TileDescriptor desc = new TileDescriptor(zoom, tile);

         if (_cachingProvider.HasTile(desc))
         {
            return _cachingProvider.GetTile(desc);
         }
         else if(!_enqueuedTiles.Contains(desc))
         {
            DownloadJob job = new DownloadJob(_mapProvider.GetTileUrl(GetNextServer(), desc.Zoom, desc.Tile), desc);
            job.OnFinished += Job_OnFinished;
            job.OnFailed += Job_OnFailed;

            _enqueuedTiles.Add(desc);


            _download.AddJob(job);
         }

         return null;
      }

      /// <summary>
      /// Called if downloading fails. Removes the tile from the download queue, so
      /// it will be retried downloading it again.
      /// </summary>
      /// <param name="sender">Event raiser.</param>
      /// <param name="info">Info object passed with the download job</param>
      private void Job_OnFailed(object sender, object info)
      {
         _enqueuedTiles.Remove((TileDescriptor) info);
      }

      /// <summary>
      /// Called when the download job finishes.
      /// If the download job finishes, the memorystream containing the downloaded data is written
      /// to the cache provider and the tile is removed from the enqueued downloads list.
      /// </summary>
      /// <param name="sender">Event raiser.</param>
      /// <param name="info">Info object passed with the download job</param>
      /// <param name="stream">The stream containing the downloaded data.</param>
      private void Job_OnFinished(object sender, object info, MemoryStream stream)
      {
         _cachingProvider.AddTile((TileDescriptor)info, stream);
         _enqueuedTiles.Remove((TileDescriptor)info);
      }

      /// <summary>
      /// Returns the next server of the provider in a rotating manner. If only one server is present or 
      /// ServerCount is zero, 0 is returned.
      /// </summary>
      /// <returns>The next server to be used for downloading tiles.</returns>
      private int GetNextServer()
      {
         return _mapProvider.ServerCount > 0 ? (_serverRotator++%_mapProvider.ServerCount) : 0;
      }

      #region Map Related stuff
      /// <summary>
      /// Transforms a Point from the MapCoordinates system to the GeoCoordinates system.
      /// </summary>
      /// <param name="mapVector">A Point within the MapCoordinates system.</param>
      /// <returns>The corresponding point in the GeoCoordinates system.</returns>
      public MapPointLatLon MapPointToLatLon(MapVector mapVector)
      {
         if (mapVector.X > MapCoordinatesWidth || mapVector.Y > MapCoordinatesHeight)
         {
            throw new ArgumentException("Coordinates of the tile are out of map size.", nameof(mapVector));
         }

         double projectedlon = ((double)mapVector.X / MapCoordinatesWidth) * 360 - 180;
         double projectedlat = (-2 * ((double)mapVector.Y / MapCoordinatesHeight) + 1) * 180;

         return Provider.Projection.FromProjection(new MapPointLatLon() { Lat = projectedlat, Lon = projectedlon });
      }

      /// <summary>
      /// Transforms a point from the GeoCoordinates system to the MapCoordinates system.
      /// </summary>
      /// <param name="pt">A Point within the GeoCoordinates system.</param>
      /// <returns>The corresponding point within the MapCoordinates system.</returns>
      public MapVector LatLonToMapPoint(MapPointLatLon pt)
      {
         MapPointLatLon proj = Provider.Projection.ToProjection(pt);

         MapVector newP = new MapVector();

         double shiftedLon = (1 + (proj.Lon / 180)) / 2;
         double shiftedLat = (1 - (proj.Lat / 180)) / 2;


         int x = (int)Math.Floor(MapCoordinatesWidth * shiftedLon);
         int y = (int)Math.Floor(MapCoordinatesHeight * shiftedLat);

         newP.X = x;
         newP.Y = y;

         return newP;
      }

      public List<TileDrawInfo> GetDrawTiles()
      {
         List<TileDrawInfo> info = new List<TileDrawInfo>();

         // 1. Get center point MapCoordinates
         MapVector centerMapVector = LatLonToMapPoint(Position);

         // 2. Calculate offset from center point to topleft point in map coordinates
         int topLeftXOffset = (int)((ViewBounds.Width / 2.0) / CoordinateScale);
         int topLeftYOffset = (int)((ViewBounds.Height / 2.0) / CoordinateScale);

         // 3. Calculate the top left point in map coordinates
         MapVector topLeftMapVector = new MapVector()
         {
            // real a mod b:
            // a mod b = (a % b + b) % b
            // https://de.wikipedia.org/wiki/Division_mit_Rest#Modulo

            X = (((centerMapVector.X - topLeftXOffset) % MapCoordinatesWidth) + MapCoordinatesWidth) % MapCoordinatesWidth,
            Y = (((centerMapVector.Y - topLeftYOffset) % MapCoordinatesHeight) + MapCoordinatesHeight) % MapCoordinatesHeight
         };

         // 4. Calculate Tile X/Y directly from Map Coordiantes
         MapVector tileCoord = new MapVector()
         {
            X = (int)Math.Floor(topLeftMapVector.X / (Provider.TileSize / CoordinateScale)),
            Y = (int)Math.Floor(topLeftMapVector.Y / (Provider.TileSize / CoordinateScale))
         };

         // 5. Calculate the Top-Left point of the top-left tile
         //PointXY tileMapVector = LatLonToMapPoint(Provider.GetPointForTile(Zoom, tileCoord));

         MapVector tileMapVector = new MapVector()
         {
            X = (int)(tileCoord.X * (Provider.TileSize / CoordinateScale)),
            Y = (int)(tileCoord.Y * (Provider.TileSize / CoordinateScale))
         };

         // 6. Get the offset of the top-left-point of the top-left-tile to the top-left point of the map
         // So we know if it is outside of the viewable port.
         MapVector offset = (tileMapVector - topLeftMapVector) * CoordinateScale;

         // 7. Create a rectangle for the top-left tile
         MapRectangle imgRect = new MapRectangle(offset.X, offset.Y, Provider.TileSize, Provider.TileSize);

         // 8. The top-left tile
         TileDescriptor descriptor = new TileDescriptor(Zoom, tileCoord);

         // 9. Now iterate over all viewable tiles and add them to the list
         // Handling the source and destination rectangles is done by special functions.

         int currentTileY = descriptor.Tile.Y;
         int currentTileX = descriptor.Tile.X;
         int currentRectY = imgRect.Y;
         int currentRectX = imgRect.X;

         int tileCount = Provider.GetTileCount(Zoom);

         while (currentRectX < ViewBounds.Width)
         {
            while (currentRectY < ViewBounds.Height)
            {
               TileDescriptor desc = new TileDescriptor(descriptor.Zoom, currentTileX, currentTileY);

               // 10. Create rectangle of the "full" tile, might lap over the borders of the viewport, i.e.
               // having negative offsets here.
               MapRectangle cRect = new MapRectangle(currentRectX, currentRectY, Provider.TileSize, Provider.TileSize);

               // 11. Get the part of the tile that is being rendered.
               MapRectangle tileSrcRect = GetSourceRectangle(cRect);

               // 12. Get the position and the size in the viewport where the tile will be rendered.
               MapRectangle viewDstRect = GetDestRectangle(cRect);

               TileDrawInfo i = new TileDrawInfo();
               i.Tile = new TileDescriptor(Zoom, currentTileX, currentTileY);
               i.SourceRectangle = tileSrcRect;
               i.DestinationRectangle = viewDstRect;

               info.Add(i);

               currentRectY += Provider.TileSize;
               currentTileY = (currentTileY + 1) % tileCount;
            }

            currentRectY = imgRect.Y;
            currentTileY = descriptor.Tile.Y;

            currentTileX = (currentTileX + 1) % tileCount;
            currentRectX += Provider.TileSize;
         }

         return info;
      }

      /// <summary>
      /// Returns the rectangle of a tile that will be rendered within the given rectangle.
      /// The rectangle passed is assumed in a viewport of (0, 0, Width, Height), even if the "real" viewport is at a different
      /// location. The rectangle returned will be the portion of the tile that is taken to render the specific tile.
      /// </summary>
      /// <param name="srcRect">The rectangle of the viewport as described above.</param>
      /// <returns>The rectangle describing the portion of the tile being rendered.</returns>
      private MapRectangle GetSourceRectangle(MapRectangle srcRect)
      {
         MapRectangle newRect = new MapRectangle();

         if (srcRect.X < 0)
         {
            newRect.X = -srcRect.X; // since offset is negative, this gets positive
            newRect.Width = srcRect.Width + srcRect.X; // + - = -!
         }
         else if (srcRect.X + srcRect.Width >= ViewBounds.Width)
         {
            newRect.X = 0;
            newRect.Width = ViewBounds.Width - srcRect.X;
         }
         else
         {
            newRect.X = 0;
            newRect.Width = srcRect.Width;
         }

         if (srcRect.Y < 0)
         {
            newRect.Y = -srcRect.Y; // since offset is negative, this gets positive
            newRect.Height = srcRect.Height + srcRect.Y; // + - = -!
         }
         else if (srcRect.Y + srcRect.Height >= ViewBounds.Height)
         {
            newRect.Y = 0;
            newRect.Height = ViewBounds.Height - srcRect.Y;
         }
         else
         {
            newRect.Y = 0;
            newRect.Height = srcRect.Height;
         }

         return newRect;
      }

      /// <summary>
      /// Gets the part of the viewport a tile will be rendered to.
      /// The input is a rectangle from a viewport at (0, 0, width, height). This rectangle
      /// is transformed to a rectangle within (viewX, viewY, width, height). Rectangled at the border of the viewport are
      /// clipped.
      /// </summary>
      /// <param name="origRect">A rectangle of the viewport as described above.</param>
      /// <returns>The part of the ViewBounds where the tile will be rendered to.</returns>
      private MapRectangle GetDestRectangle(MapRectangle origRect)
      {
         MapRectangle newRect = new MapRectangle();

         if (origRect.X < 0)
         {
            newRect.X = ViewBounds.X;
            newRect.Width = origRect.Width + origRect.X; // + - = -!
         }
         else if (origRect.X + origRect.Width >= ViewBounds.Width)
         {
            newRect.X = ViewBounds.X + origRect.X;
            newRect.Width = ViewBounds.Width - origRect.X;
         }
         else
         {
            newRect.X = ViewBounds.X + origRect.X;
            newRect.Width = origRect.Width;
         }

         if (origRect.Y < 0)
         {
            newRect.Y = ViewBounds.Y; 
            newRect.Height = origRect.Height + origRect.Y; // + - = -!
         }
         else if (origRect.Y + origRect.Height > ViewBounds.Height)
         {
            newRect.Y = ViewBounds.Y + origRect.Y;
            newRect.Height = ViewBounds.Height - origRect.Y;
         }
         else
         {
            newRect.Y = ViewBounds.Y + origRect.Y;
            newRect.Height = origRect.Height;
         }

         return newRect;
      }
      #endregion
   }
}
