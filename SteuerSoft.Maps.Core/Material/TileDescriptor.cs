using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteuerSoft.Maps.Core.Material
{
   /// <summary>
   /// This struct represents the 3d-Coordinate of a tile.
   /// Each tile can be identified ba a Zoom level it is in and X/Y Coordinates within that zoom level.
   /// 
   /// All this information is stored within this struct.
   /// </summary>
   public struct TileDescriptor : IEquatable<TileDescriptor>
   {
      /// <summary>
      /// The Zoom level the tile is in.
      /// </summary>
      public int Zoom { get; set; }

      /// <summary>
      /// The X/Y Coordinates of the tile within the specified zoom level.
      /// </summary>
      public MapPoint Tile { get; set; }

      /// <summary>
      /// Creates a new tile descriptor using raw coordinates.
      /// </summary>
      /// <param name="zoom">The zoom level.</param>
      /// <param name="x">X-Coordinate within the zoom level.</param>
      /// <param name="y">Y-Coordinate within the zoom level.</param>
      public TileDescriptor(int zoom, int x, int y)
         :this(zoom, new MapPoint() { X = x, Y = y})
      {
         
      }

      /// <summary>
      /// Creates a new descriptor using a zoom level and a point.
      /// </summary>
      /// <param name="zoom">The zoom level.</param>
      /// <param name="tile">The Coordinates of the tile within the zoom level.</param>
      public TileDescriptor(int zoom, MapPoint tile)
      {
         Zoom = zoom;
         Tile = tile;
      }

      public bool Equals(TileDescriptor other)
      {
         return Zoom == other.Zoom && Tile.Equals(other.Tile);
      }

      public override bool Equals(object obj)
      {
         return obj is TileDescriptor && Equals((TileDescriptor) obj);
      }

      public override int GetHashCode()
      {
         return 331 + Zoom.GetHashCode()*389 + Tile.GetHashCode();
      }
   }
}
