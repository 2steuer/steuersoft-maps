using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteuerSoft.Maps.Core.Material;

namespace SteuerSoft.Maps.Core.Interfaces
{
   /// <summary>
   /// Classes implementing this interface represent a specific tiled map provider.
   /// 
   /// Classes inheriting this interfaces should be implemented using singleton pattern, since
   /// the class only stores static data and usually has no instance fields.
   /// </summary>
   public interface IMapProvider
   {
      /// <summary>
      /// The human readable name of the provider.
      /// </summary>
      string Name { get; }

      /// <summary>
      /// A name used internally, e.g. for caching.
      /// </summary>
      string InternalName { get; }

      /// <summary>
      /// A copyright string that shall be rendered on displayed maps.
      /// </summary>
      string Copyright { get; }

      /// <summary>
      /// The tile size in pixels. Tiles shall be as high as wide.
      /// </summary>
      int TileSize { get; }

      /// <summary>
      /// Minimum zoom level supported by the provider.
      /// </summary>
      int MinZoom { get; }

      /// <summary>
      /// Maximum zoom level supported by the provider.
      /// </summary>
      int MaxZoom { get; }

      /// <summary>
      /// Maximum threads allowed downloading tiles at the same moment.
      /// </summary>
      int MaxDownloadThreads { get; }

      /// <summary>
      /// Number of different servers. Should be at least 1.
      /// </summary>
      int ServerCount { get; }

      /// <summary>
      /// Projection used by the provider.
      /// </summary>
      IProjection Projection { get; }

      /// <summary>
      /// Gets a tile (X/Y) for a Geo-Coordinate at a given point.
      /// 
      /// See https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#X_and_Y
      /// </summary>
      /// <param name="zoom">The zoom level hat which the tile coordinates shall be calculated.</param>
      /// <param name="point">The Point in Geo-Coordinates of whcih the tile shall be calculated.</param>
      /// <returns>The X/Y coordinates of the tile containing the given geo-coordinate.</returns>
      MapPoint GetTileForPoint(int zoom, MapPointLatLon point);

      /// <summary>
      /// Returns the Lat/Lon geo-coordinates for a tile
      /// </summary>
      /// <param name="zoom">The zoom level the tile is at</param>
      /// <param name="tile">The tile coordinates in X/Y</param>
      /// <returns>The Geo-Point the top-left corner of the tile is at.</returns>
      MapPointLatLon GetPointForTile(int zoom, MapPoint tile);

      /// <summary>
      /// Returns the number of tiles in each direction (x and y) on the given zoom level.
      /// </summary>
      /// <param name="zoom">The zoom level.</param>
      /// <returns>The number of Tiles in each direction for the given zoom level.</returns>
      int GetTileCount(int zoom);

      /// <summary>
      /// Returns the URL to the tile on the given server number.
      /// </summary>
      /// <param name="server">The server number. Should be rotated by the caller.</param>
      /// <param name="zoom">The zoom level the tile is at.</param>
      /// <param name="tile">The X/Y of the tile at the given zoom level.</param>
      /// <returns>The URL to the given tile.</returns>
      string GetTileUrl(int server, int zoom, MapPoint tile);
   }
}
