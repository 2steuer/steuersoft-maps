using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteuerSoft.Maps.Core.Material
{
   /// <summary>
   /// A struct representing a point in integer X/Y coordinates.
   /// </summary>
   public struct MapPoint : IEquatable<MapPoint>
   {
      /// <summary>
      /// The X Coordinate
      /// </summary>
      public int X { get; set; }

      /// <summary>
      /// The Y coodinate.
      /// </summary>
      public int Y { get; set; }


      public override bool Equals(object obj) => obj is MapPoint && Equals((MapPoint) obj);

      public bool Equals(MapPoint other)
      {
         return X == other.X &&
                Y == other.Y;
      }

      public override int GetHashCode()
      {
         return 17 + X.GetHashCode() * 23 + Y.GetHashCode() * 31;
      }

      public static MapPoint operator -(MapPoint a, MapPoint b)
      {
         return new MapPoint() {X = a.X - b.X, Y = a.Y - b.Y};
      }

      public static MapPoint operator +(MapPoint a, MapPoint b)
      {
         return new MapPoint() { X = a.X + b.X, Y = a.Y + b.Y };
      }

      public static MapPoint operator *(MapPoint a, double scale)
      {
         return new MapPoint() {X = (int)(a.X * scale), Y = (int)(a.Y * scale)};
      }

      public static MapPoint operator *(double scale, MapPoint a)
      {
         return a * scale;
      }

      public static MapPoint operator /(MapPoint a, double div)
      {
         return new MapPoint() {X = (int)(a.X / div), Y = (int)(a.Y / div)};
      }
   }
}
