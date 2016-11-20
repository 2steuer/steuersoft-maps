using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteuerSoft.Maps.Core.Material;

namespace SteuerSoft.Maps.Core.Interfaces
{
   /// <summary>
   /// Interface for tile caching providers.
   /// </summary>
   public interface ITileCachingProvider : IDisposable
   {
      /// <summary>
      /// The Sub-Cache to be used. Useful when often changing between Map providers so we can 
      /// easily seperate the data from another.
      /// </summary>
      string SubCache { get; set; }

      /// <summary>
      /// Checks if a tile exists within the caching provider.
      /// </summary>
      /// <param name="tile">The tile that shall be checked</param>
      /// <returns>true if the Tile is available in the caching provider, false otherwise</returns>
      bool HasTile(TileDescriptor tile);

      /// <summary>
      /// Returns a stream to the given tile's data.
      /// The Stream returned should be disposed after It's usage.
      /// </summary>
      /// <param name="tile">The Tile</param>
      /// <returns>A Stream to the data of the given tile file. Returns null if the tile was not found.</returns>
      Stream GetTile(TileDescriptor tile);

      /// <summary>
      /// Adds a tile to the cache. The stream is completely copied and may be disposed after the function call.
      /// </summary>
      /// <param name="tile">The Tile that shall be added.</param>
      /// <param name="data">A Stream to the tile's data.</param>
      void AddTile(TileDescriptor tile, Stream data);

   }
}
