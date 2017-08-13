using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteuerSoft.Osm.StreetNetwork.Material
{
   public class Path
   {
      public List<Waypoint> Waypoints { get; } = new List<Waypoint>();

      public double Length
      {
         get
         {
            if (Waypoints.Count <= 1)
            {
               return 0;
            }

            double length = 0;
            Waypoint current = Waypoints[0];

            for (int i = 1; i < Waypoints.Count; i++)
            {
               Waypoint next = Waypoints[i];
               length += current.DistanceTo(next);
               current = next;
            }

            return length;
         }
      }
   }
}
