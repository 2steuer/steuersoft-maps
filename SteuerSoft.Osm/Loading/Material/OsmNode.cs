using System.Collections.Generic;

namespace SteuerSoft.Osm.Loading.Material
{
   public class OsmNode
   {
      public long Id { get; set; }
      public double Lat { get; set; }
      public double Lon { get; set; }

      public Dictionary<string, string> Tags { get; } = new Dictionary<string, string>();

      public OsmNode(long id, double lat, double lon)
      {
         Id = id;
         Lat = lat;
         Lon = lon;
      }

      public OsmNode()
         :this(0, 0.0, 0.0)
      {
         
      }
   }
}
