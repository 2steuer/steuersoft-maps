using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteuerSoft.Osm.Loading;
using SteuerSoft.Osm.Material;
using SteuerSoft.Osm.PathFinding.Algorithms;
using SteuerSoft.Osm.PathFinding.Algorithms.Interface;
using SteuerSoft.Osm.StreetNetwork.Material;

namespace SteuerSoft.Osm.StreetNetwork
{
   public class OsmStreetSystem
   {
      private static string[] _highwayWhitelist =
      {
         "motorway",
         "trunk",
         "primary",
         "secondary",
         "tertiary",
         "unclassified",
         "residential",
         "service",
         "motorway_link",
         "trunk_link",
         "primary_link",
         "secondary_link",
         "tertiary_link"
      };

      public Dictionary<long, Waypoint> WayPoints { get; } = new Dictionary<long, Waypoint>();

      private IPathfindingAlgorithm _pathFinder = new DijkstraAlgorithm();

      private OsmStreetSystem()
      {
         
      }

      private Waypoint GetWaypointForNode(OsmNode node)
      {
         if (WayPoints.ContainsKey(node.Id))
         {
            return WayPoints[node.Id];
         }
         else
         {
            Waypoint nw = new Waypoint(node);
            WayPoints.Add(node.Id, nw);
            return nw;
         }
      }

      public static OsmStreetSystem Build(OsmLoader data)
      {
         OsmStreetSystem newSystem = new OsmStreetSystem();

         var ways = data.Ways.Values.Where(w => _highwayWhitelist.Contains(w.GetTag("highway", "-")));

         foreach (var way in ways)
         {
            bool oneWay = (way.GetTag("oneway", "no") == "yes")
               || (way.GetTag("junction", "-") == "roundabout");

            Waypoint lastPoint = newSystem.GetWaypointForNode(way.Nodes[0]);

            for (int i = 1; i < way.Nodes.Count; i++)
            {
               var current = newSystem.GetWaypointForNode(way.Nodes[i]);

               lastPoint.ConnectTo(current);

               if (!oneWay)
               {
                  current.ConnectTo(lastPoint);
               }

               lastPoint = current;
            }
         }

         return newSystem;
      }

      public Path FindPath(Waypoint start, Waypoint end)
      {
         return _pathFinder.FindPath(start, end);
      }
   }
}
