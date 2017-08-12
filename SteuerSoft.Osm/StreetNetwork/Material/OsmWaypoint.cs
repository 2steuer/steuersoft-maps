using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteuerSoft.Osm.Loading.Material;

namespace SteuerSoft.Osm.StreetNetwork.Material
{
   public class OsmWaypoint
   {
      OsmNode _node;

      public long Id => _node.Id;
      public double Lat => _node.Lat;
      public double Lon => _node.Lon;

      public HashSet<OsmConnection> Connections { get; } = new HashSet<OsmConnection>();

      public OsmWaypoint(OsmNode node)
      {
         _node = node;
      }
   }
}
