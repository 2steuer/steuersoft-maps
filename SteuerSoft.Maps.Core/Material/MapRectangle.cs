using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteuerSoft.Maps.Core.Material
{
   /// <summary>
   /// This struct represents a rectangle in integral lengths.
   /// </summary>
   public struct MapRectangle
   {
      /// <summary>
      /// The Location of the rectangle.
      /// </summary>
      public MapVector Location;

      /// <summary>
      /// The dimensions of the rectangle.
      /// </summary>
      public MapSize Size;

      /// <summary>
      /// The X Coordinates of the top-left corner of the rectangle.
      /// </summary>
      public int X { get { return Location.X; } set { Location.X = value; } }

      /// <summary>
      /// The Y Coordinates of the top-left corner of the rectangle.
      /// </summary>
      public int Y { get { return Location.Y; } set { Location.Y = value; } }

      /// <summary>
      /// Width of the rectangle
      /// </summary>
      public int Width { get { return Size.Width; } set { Size.Width = value; } }

      /// <summary>
      /// The Height of the rectangle.
      /// </summary>
      public int Height { get { return Size.Height; } set { Size.Height = value; } }

      /// <summary>
      /// Creates a new rectangle.
      /// </summary>
      /// <param name="x">X-Coordinate</param>
      /// <param name="y">Y-Corodinate</param>
      /// <param name="width">Width of the rectangle</param>
      /// <param name="height">Height of the rectangle.</param>
      public MapRectangle(int x, int y, int width, int height)
      {
         Location = new MapVector();
         Size = new MapSize();

         X = x;
         Y = y;
         Width = width;
         Height = height;
      }

      public bool Contains(MapVectorD vect, bool onBorder = true)
      {
         if (onBorder)
         {
            return (vect.X >= X)
                && (vect.Y >= Y)
                && (vect.X <= X + Width)
                && (vect.Y <= Y + Height);
         }
         else
         {
            return (vect.X > X)
                && (vect.Y > Y)
                && (vect.X < X + Width)
                && (vect.Y < Y + Height);
         }
         
      }
   }
}
