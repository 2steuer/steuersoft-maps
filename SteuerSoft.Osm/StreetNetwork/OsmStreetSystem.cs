using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteuerSoft.Osm.Loading;
using SteuerSoft.Osm.Loading.Material;
using SteuerSoft.Osm.StreetNetwork.Material;

namespace SteuerSoft.Osm.StreetNetwork
{
   public class OsmStreetSystem
   {
      public Dictionary<long, OsmWaypoint> WayPoints { get; } = new Dictionary<long, OsmWaypoint>();

      private OsmStreetSystem()
      {
         
      }

      public static OsmStreetSystem Build(OsmLoader data)
      {
         OsmStreetSystem newSystem = new OsmStreetSystem();

         var ways = data.Ways.Where(wp => wp.Value.Tags.ContainsKey("highway"));

         foreach (var wayE in ways)
         {
            var way = wayE.Value;

            bool oneWay = way.Tags.ContainsKey("oneway") && way.Tags["oneway"] == "yes";

            OsmWaypoint oldNode = null;

            foreach (var node in way.Nodes)
            {
               OsmWaypoint newWp = new OsmWaypoint(node);

               if (oldNode != null)
               {
                  oldNode.Connections.Add(new OsmConnection(newWp));

                  if (!oneWay)
                  {
                     newWp.Connections.Add(new OsmConnection(oldNode));
                  }
               }

               oldNode = newWp;

               if (!newSystem.WayPoints.ContainsKey(newWp.Id))
               {
                  newSystem.WayPoints.Add(newWp.Id, newWp);
               }
               
            }
         }

         return newSystem;
         
      }
   }
}
