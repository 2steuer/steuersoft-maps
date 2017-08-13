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
         "motorway_link",
         "trunk_link",
         "primary_link",
         "secondary_link",
         "tertiary_link",
         "service"
      };

      private static Dictionary<string, string> _defaultMaxSpeeds = new Dictionary<string, string>()
      {
         {"motorway", "130"},
         {"trunk", "120"},
         {"primary", "100"},
         {"secondary", "70"},
         {"tertiary", "50"},
         {"unclassified", "30"},
         {"residential", "10"},
         {"motorway_link", "130"},
         {"trunk_link", "120"},
         {"primary_link", "100" },
         {"secondary_link", "70"},
         {"tertiary_link", "50"},
         {"service", "20"}
      };

      private static int _maxSpeed = 130;
      
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

            var conInfo = MakeConnectionInfo(way);

            for (int i = 1; i < way.Nodes.Count; i++)
            {
               var current = newSystem.GetWaypointForNode(way.Nodes[i]);

               lastPoint.ConnectTo(current, conInfo);

               if (!oneWay)
               {
                  current.ConnectTo(lastPoint, conInfo);
               }

               lastPoint = current;
            }
         }

         return newSystem;
      }

      private static ConnectionInfo MakeConnectionInfo(OsmWay way)
      {
         ConnectionInfo i = new ConnectionInfo();

         if (!way.HasTag("maxspeed"))
         {
            i.MaxSpeed = double.Parse(_defaultMaxSpeeds[way.GetTag("highway")]);
         }
         else
         {
            string speed = way.GetTag("maxspeed");

            if (speed == "none")
            {
               i.MaxSpeed = _maxSpeed;

            }
            else
            {
               i.MaxSpeed = double.Parse(speed);
            }
         }

         return i;
      }

      public void SetPathFinder(IPathfindingAlgorithm finder)
      {
         _pathFinder = finder;
      }

      public Path FindPath(Waypoint start, Waypoint end)
      {
         return _pathFinder.FindPath(start, end);
      }
   }
}
