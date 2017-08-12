using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using SteuerSoft.Osm.Loading.Material;

namespace SteuerSoft.Osm.Loading
{
   public class OsmLoader
   {
      public string File { get; private set; }

      public Dictionary<long, OsmNode> Nodes { get; } = new Dictionary<long, OsmNode>();
      public Dictionary<long, OsmWay> Ways { get; } = new Dictionary<long, OsmWay>();

      private OsmLoader()
      {
         
      }

      private void Parse()
      {
         XDocument doc = XDocument.Load(File);
         XElement root = doc.Root;

         foreach (var xElement in root.Elements("node"))
         {
            long id = long.Parse(xElement.Attribute("id").Value);
            double lat = double.Parse(xElement.Attribute("lat").Value, CultureInfo.InvariantCulture);
            double lng = double.Parse(xElement.Attribute("lon").Value, CultureInfo.InvariantCulture);

            OsmNode newNode = new OsmNode(id, lat, lng);

            foreach (var tagE in xElement.Elements("tag"))
            {
               newNode.Tags.Add(tagE.Attribute("k").Value, tagE.Attribute("v").Value);
            }

            Nodes.Add(id, newNode);
         }

         foreach (var xElement in root.Elements("way"))
         {
            OsmWay newWay = new OsmWay();
            newWay.Id = long.Parse(xElement.Attribute("id").Value);

            foreach (var element in xElement.Elements("nd"))
            {
               long nodeId = long.Parse(element.Attribute("ref").Value);
               newWay.Nodes.Add(Nodes[nodeId]);
            }

            foreach (var tagE in xElement.Elements("tag"))
            {
               newWay.Tags.Add(tagE.Attribute("k").Value, tagE.Attribute("v").Value);
            }

            Ways.Add(newWay.Id, newWay);
         }
      }

      public static OsmLoader Load(string fileName)
      {
         OsmLoader ret = new OsmLoader();
         ret.File = fileName;
         ret.Parse();

         return ret;
      }
   }
}
