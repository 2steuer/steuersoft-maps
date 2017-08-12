using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteuerSoft.Osm.StreetNetwork.Material
{
   public class OsmConnection
   {
      public OsmWaypoint Target { get; set; }

      public OsmConnection(OsmWaypoint target)
      {
         Target = target;
      }
   }
}
