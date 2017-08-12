using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;
using SteuerSoft.Osm.Material;

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
            var newNode = OsmNode.FromXElement(xElement);
            Nodes.Add(newNode.Id, newNode);
         }

         foreach (var xElement in root.Elements("way"))
         {
            var newWay = OsmWay.FromXElement(xElement, Nodes);
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
