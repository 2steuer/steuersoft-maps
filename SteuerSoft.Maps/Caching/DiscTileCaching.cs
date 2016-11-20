using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteuerSoft.Maps.Core.Interfaces;
using SteuerSoft.Maps.Core.Material;

namespace SteuerSoft.Maps.Caching
{
   /// <summary>
   /// A cache provider that stores the tiles in files on the disc.
   /// There is no caching in memory, all files are always read from the hard disc. This might get 
   /// slow when there are a lot of tiles needed.
   /// </summary>
   public class DiscTileCaching : ITileCachingProvider
   {
      /// <summary>
      /// The main cache directory.
      /// </summary>
      private string _cacheDir;

      public string SubCache { get; set; }

      /// <summary>
      /// Initialises a new instance of the disc cache provider.
      /// </summary>
      /// <param name="cacheDir">The main cache directory, under which all subcaches are
      /// also stored in subdirectories.</param>
      public DiscTileCaching(string cacheDir)
      {
         _cacheDir = cacheDir;

         SubCache = "default";

         if (!Directory.Exists(_cacheDir))
         {
            Directory.CreateDirectory(_cacheDir);
         }
      }

      public void Dispose()
      {
         
      }

      public bool HasTile(TileDescriptor tile)
      {
         return File.Exists(GetFilePath(tile));
      }

      public Stream GetTile(TileDescriptor tile)
      {
         try
         {
            return File.Open(GetFilePath(tile), FileMode.Open, FileAccess.Read, FileShare.Read);
         }
         catch (IOException)
         {
            return null;
         }
         
      }

      public void AddTile(TileDescriptor tile, Stream data)
      {
         string filepath = GetFilePath(tile);
         string dir = Path.GetDirectoryName(filepath);

         if (!Directory.Exists(dir))
         {
            Directory.CreateDirectory(dir);
         }

         var file = File.Create(filepath);
         data.CopyTo(file);
         file.Close();
      }

      private string GetFilePath(TileDescriptor tile)
      {
         return Path.Combine(_cacheDir, SubCache, tile.Zoom.ToString(), tile.Tile.X.ToString(), $"{tile.Tile.Y}.png");
      }
   }
}
