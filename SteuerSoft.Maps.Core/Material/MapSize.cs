using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteuerSoft.Maps.Core.Material
{
   /// <summary>
   /// This struct represents a Size in integral Width/Height values.
   /// </summary>
   public struct MapSize
   {
      /// <summary>
      /// The Width.
      /// </summary>
      public int Width;

      /// <summary>
      /// The height.
      /// </summary>
      public int Height;


      /// <summary>
      /// Creates a new instance of the Size struct.
      /// </summary>
      /// <param name="width">Width</param>
      /// <param name="height">Height</param>
      public MapSize(int width, int height)
      {
         Width = width;
         Height = height;
      }
   }
}
