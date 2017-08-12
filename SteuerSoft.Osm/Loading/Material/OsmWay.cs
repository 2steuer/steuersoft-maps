using System.Collections.Generic;

namespace SteuerSoft.Osm.Loading.Material
{
   public class OsmWay
   {
      public long Id { get; set; }
      public List<OsmNode> Nodes { get; } = new List<OsmNode>();
      public Dictionary<string, string> Tags { get; } = new Dictionary<string, string>();
   }
}
