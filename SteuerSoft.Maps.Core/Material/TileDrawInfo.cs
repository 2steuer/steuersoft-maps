using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteuerSoft.Maps.Core.Material
{
   /// <summary>
   /// This struct stores data about a tile to be drawn.
   /// </summary>
   public struct TileDrawInfo
   {
      /// <summary>
      /// The tile that will be drawn.
      /// </summary>
      public TileDescriptor Tile;

      /// <summary>
      /// The part of the tile that shall be drawn. Used when the tile overlaps borders of the viewport.
      /// </summary>
      public MapRectangle SourceRectangle;

      /// <summary>
      /// The part of the viewport where the tile shall be drawn.
      /// </summary>
      public MapRectangle DestinationRectangle;
   }
}
