using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteuerSoft.Maps.Core.Material
{
   /// <summary>
   /// A struct representing a point in double precision floating point X/Y coordinates.
   /// </summary>
   public struct MapVectorD : IEquatable<MapVectorD>
   {
      /// <summary>
      /// The X Coordinate
      /// </summary>
      public double X { get; set; }

      /// <summary>
      /// The Y coodinate.
      /// </summary>
      public double Y { get; set; }

      public MapVectorD(MapVector source)
      {
         X = source.X;
         Y = source.Y;
      }

      public override bool Equals(object obj) => obj is MapVector && Equals((MapVector)obj);

      public bool Equals(MapVectorD other)
      {
         return X == other.X &&
                Y == other.Y;
      }

      public override int GetHashCode()
      {
         return 17 + X.GetHashCode() * 23 + Y.GetHashCode() * 31;
      }

      public static MapVectorD operator -(MapVectorD a, MapVectorD b)
      {
         return new MapVectorD() { X = a.X - b.X, Y = a.Y - b.Y };
      }

      public static MapVectorD operator +(MapVectorD a, MapVectorD b)
      {
         return new MapVectorD() { X = a.X + b.X, Y = a.Y + b.Y };
      }

      public static MapVectorD operator *(MapVectorD a, double scale)
      {
         return new MapVectorD() { X = (a.X * scale), Y = (a.Y * scale) };
      }

      public static MapVectorD operator *(double scale, MapVectorD a)
      {
         return a * scale;
      }

      public static MapVectorD operator /(MapVectorD a, double div)
      {
         return new MapVectorD() { X = (a.X / div), Y = (a.Y / div) };
      }
   }
}
