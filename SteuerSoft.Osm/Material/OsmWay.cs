using System.Collections.Generic;
using System.Xml.Linq;
using SteuerSoft.Osm.Material.Base;

namespace SteuerSoft.Osm.Material
{
   public class OsmWay : OsmTaggableBase
   {
      public long Id { get; set; }
      public List<OsmNode> Nodes { get; } = new List<OsmNode>();

      public static OsmWay FromXElement(XElement ele, Dictionary<long, OsmNode> nodes)
      {
         OsmWay way = new OsmWay();
         way.ParseTags(ele);

         way.Id = long.Parse(ele.Attribute("id").Value);

         foreach (var element in ele.Elements("nd"))
         {
            long nodeId = long.Parse(element.Attribute("ref").Value);
            way.Nodes.Add(nodes[nodeId]);
         }

         return way;
      }
   }
}
