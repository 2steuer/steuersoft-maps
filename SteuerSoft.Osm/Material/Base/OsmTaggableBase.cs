using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SteuerSoft.Osm.Material.Base
{
   public abstract class OsmTaggableBase
   {
      public Dictionary<string, string> Tags { get; } = new Dictionary<string, string>();

      public bool HasTag(string key) => Tags.ContainsKey(key);

      public string GetTag(string key)
      {
         return Tags[key];
      }

      public string GetTag(string key, string defaultValue)
      {
         return HasTag(key) ? Tags[key] : defaultValue;
      }

      protected void ParseTags(XElement element)
      {
         foreach (XElement xe in element.Elements("tag"))
         {
            var k = xe.Attribute("k")?.Value ?? string.Empty;
            var v = xe.Attribute("v")?.Value;

            if (!string.IsNullOrEmpty(k))
            {
               Tags.Add(k, v);
            }
         }
      }
   }
}
