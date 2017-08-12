using System.Globalization;
using System.Xml.Linq;
using SteuerSoft.Osm.Material.Base;

namespace SteuerSoft.Osm.Material
{
   public class OsmNode : OsmTaggableBase
   {
      public long Id { get; set; }
      public double Lat { get; set; }
      public double Lon { get; set; }

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

      public static OsmNode FromXElement(XElement ele)
      {
         OsmNode newNode = new OsmNode()
         {
            Id = long.Parse(ele.Attribute("id").Value),
            Lat = double.Parse(ele.Attribute("lat").Value, CultureInfo.InvariantCulture),
            Lon = double.Parse(ele.Attribute("lon").Value, CultureInfo.InvariantCulture)
         };

         newNode.ParseTags(ele);

         return newNode;
      }
   }
}
