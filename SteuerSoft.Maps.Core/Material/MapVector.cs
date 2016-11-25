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
   public struct MapVector : IEquatable<MapVector>
   {
      /// <summary>
      /// The X Coordinate
      /// </summary>
      public int X { get; set; }

      /// <summary>
      /// The Y coodinate.
      /// </summary>
      public int Y { get; set; }


      public override bool Equals(object obj) => obj is MapVector && Equals((MapVector) obj);

      public bool Equals(MapVector other)
      {
         return X == other.X &&
                Y == other.Y;
      }

      public override int GetHashCode()
      {
         return 17 + X.GetHashCode() * 23 + Y.GetHashCode() * 31;
      }

      public static MapVector operator -(MapVector a, MapVector b)
      {
         return new MapVector() {X = a.X - b.X, Y = a.Y - b.Y};
      }

      public static MapVector operator +(MapVector a, MapVector b)
      {
         return new MapVector() { X = a.X + b.X, Y = a.Y + b.Y };
      }

      public static MapVector operator *(MapVector a, double scale)
      {
         return new MapVector() {X = (int)(a.X * scale), Y = (int)(a.Y * scale)};
      }

      public static MapVector operator *(double scale, MapVector a)
      {
         return a * scale;
      }

      public static MapVector operator /(MapVector a, double div)
      {
         return new MapVector() {X = (int)(a.X / div), Y = (int)(a.Y / div)};
      }
   }
}
