using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteuerSoft.Maps.Core.Interfaces;
using SteuerSoft.Maps.Core.Material;
using SteuerSoft.Maps.Projections;

namespace SteuerSoft.Maps.Providers
{
   /// <summary>
   /// Map Provider for the original OpenStreetMap tile server.
   /// </summary>
   public class OsmProvider : IMapProvider
   {
      private static OsmProvider _instance;

      public string Name => "OpenStreetMap";
      public string InternalName => "osm_orig";
      public string Copyright => $"(c) {DateTime.Now.Year} OpenStreetMap.org";

      public int TileSize => 256;
      public int MinZoom => 0;
      public int MaxZoom => 19;

      private readonly string[] _servers = { "a", "b", "c" };

      public int ServerCount => _servers.Length;

      private IProjection _projection;

      public IProjection Projection => _projection;

      public int MaxDownloadThreads => 2;

      private OsmProvider()
      {
         _projection = MercatorProjection.GetInstance();  
      }

      public string GetTileUrl(int server, int zoom, MapVector tile)
      {
         return $"http://{(server < _servers.Length ? _servers[server] : _servers[0])}.tile.openstreetmap.org/{zoom}/{tile.X}/{tile.Y}.png";
      }

      public static OsmProvider GetInstance()
      {
         if (_instance == null)
         {
            _instance = new OsmProvider();
         }

         return _instance;
      }

      /// <summary>
      /// Gets a tile (X/Y) for a Geo-Coordinate at a given point.
      /// 
      /// See https://wiki.openstreetmap.org/wiki/Slippy_map_tilenames#X_and_Y
      /// </summary>
      /// <param name="zoom">The zoom level hat which the tile coordinates shall be calculated.</param>
      /// <param name="point">The Point in Geo-Coordinates of whcih the tile shall be calculated.</param>
      /// <returns>The X/Y coordinates of the tile containing the given geo-coordinate.</returns>
      public MapVector GetTileForPoint(int zoom, MapPointLatLon point)
      {
         if (zoom < MinZoom || zoom > MaxZoom)
         {
            throw new ArgumentException($"zoom is not within MinZoom and MaxZoom {zoom}", nameof(zoom));
         } 

         var projPoint = _projection.ToProjection(point);

         double shiftedLon = (1 + (projPoint.Lon/Math.Abs(_projection.MinProjectedLon)))/2;
         double shiftedLat = (1 - (projPoint.Lat/Math.Abs(_projection.MinProjectedLat)))/2;

         double n = GetTileCount(zoom);

         int x = (int) Math.Floor(n*shiftedLon);
         int y = (int) Math.Floor(n*shiftedLat);

         return new MapVector() {X = x, Y = y};
      }

      /// <summary>
      /// Returns the Lat/Lon geo-coordinates for a tile
      /// </summary>
      /// <param name="zoom">The zoom level the tile is at</param>
      /// <param name="tile">The tile coordinates in X/Y</param>
      /// <returns>The Geo-Point the top-left corner of the tile is at.</returns>
      public MapPointLatLon GetPointForTile(int zoom, MapVector tile)
      {
         double n = GetTileCount(zoom);
         if (tile.X > n || tile.Y > n)
         {
            throw new ArgumentException("Coordinates of the tile are out of the given zoom's range.", nameof(tile));
         }

         double projectedlon = (tile.X/n)*360 - 180;
         double projectedlat = (-2*(tile.Y/n) + 1)*180;

         return _projection.FromProjection(new MapPointLatLon() {Lat = projectedlat, Lon = projectedlon});
      }

      public int GetTileCount(int zoom)
      {
         return (int) Math.Pow(2, zoom);
      }
   }
}
