using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteuerSoft.Osm.StreetNetwork.Material
{
   public class Connection
   {
      public Waypoint Source { get; set; }
      public Waypoint Target { get; set; }

      public double Distance { get; set; }

      public Connection(Waypoint source, Waypoint target)
      {
         Source = source;
         Target = target;
         Distance = Source.DistanceTo(Target);
      }
   }
}
