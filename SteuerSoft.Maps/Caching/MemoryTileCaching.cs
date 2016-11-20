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
   /// A Cache provider that stores all loaded tiles within memory.
   /// Since there is no persistence, the use of this provider in production systems is strongly
   /// discouraged. 
   /// 
   /// </summary>
   public class MemoryTileCaching : ITileCachingProvider
   {
      Dictionary<string, Dictionary<TileDescriptor, MemoryStream>> _cache = new Dictionary<string, Dictionary<TileDescriptor, MemoryStream>>();

      private string _subCache;

      public string SubCache
      {
         get
         {
            return _subCache;
         }
         set
         {
            if (!_cache.ContainsKey(value))
            {
               _cache.Add(value, new Dictionary<TileDescriptor, MemoryStream>());

            }
            _subCache = value;
         }
      }

      public MemoryTileCaching()
      {
         SubCache = "default";
      }

      public bool HasTile(TileDescriptor tile)
      {
         return _cache[_subCache].ContainsKey(tile);
      }

      public Stream GetTile(TileDescriptor tile)
      {
         MemoryStream memStr = new MemoryStream();

         if (!HasTile(tile))
         {
            return null;
         }
         else
         {
            _cache[_subCache][tile].Position = 0;
            _cache[_subCache][tile].CopyTo(memStr);
         }

         return memStr;
      }

      public void AddTile(TileDescriptor tile, Stream data)
      {
         MemoryStream memStr = new MemoryStream();
         data.CopyTo(memStr);

         if (_cache[_subCache].ContainsKey(tile))
         {
            _cache[_subCache][tile].Dispose();
            _cache[_subCache][tile] = memStr;
         }
         else
         {
            _cache[_subCache].Add(tile, memStr);
         }
      }

      public void Dispose()
      {
         foreach (var dicts in _cache)
         {
            foreach (var stream in dicts.Value)
            {
               stream.Value.Dispose();
            }
         }
      }
   }
}
