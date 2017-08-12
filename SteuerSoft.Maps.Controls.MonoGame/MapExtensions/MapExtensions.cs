using Microsoft.Xna.Framework;
using SteuerSoft.Maps.Core.Material;

namespace SteuerSoft.Maps.Controls.MonoGame.MapExtensions
{
   /// <summary>
   /// This class contains extension methods for the different data types (Rectangles etc.)
   /// that must be converted between the Maps module and the MonoGame module.
   /// </summary>
   public static class MapExtensions
   {
      /// <summary>
      /// Converts a MapRectangle from the Maps module to a Rectangle from the monogame module.
      /// </summary>
      /// <param name="rect">A MapRectangle to convert</param>
      /// <returns>The MapRectangle as a Rectangle.</returns>
      public static Rectangle ToRectangle(this MapRectangle rect)
      {
         return new Rectangle(rect.X, rect.Y, rect.Width, rect.Height);
      }

      /// <summary>
      /// Converts a Rectangle from the MonoGame module to a MapRectangle from the Maps module.
      /// </summary>
      /// <param name="rect">A Rectangle to convert</param>
      /// <returns>The Rectangle as a MapRectangle.</returns>
      public static MapRectangle ToMapRectangle(this Rectangle rect)
      {
         return new MapRectangle(rect.X, rect.Y, rect.Width, rect.Height);
      }

      public static Vector2 ToVector2(this MapVectorD vect)
      {
         return new Vector2((float)vect.X, (float)vect.Y);
      }

      public static Vector2 ToVector2(this MapVector vect)
      {
         return new Vector2((float)vect.X, (float)vect.Y);
      }
   }
}
