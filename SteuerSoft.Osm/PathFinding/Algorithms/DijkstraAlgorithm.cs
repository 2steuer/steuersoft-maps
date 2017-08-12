using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteuerSoft.Osm.PathFinding.Algorithms.Interface;
using SteuerSoft.Osm.StreetNetwork.Material;

namespace SteuerSoft.Osm.PathFinding.Algorithms
{
   class DijkstraAlgorithm : IPathfindingAlgorithm
   {
      public Path FindPath(Waypoint start, Waypoint end)
      {
         bool found = false;
         Dictionary<Waypoint, double> distances = new Dictionary<Waypoint, double>();
         HashSet<Waypoint> visited = new HashSet<Waypoint>();
         Dictionary<Waypoint, Waypoint> pres = new Dictionary<Waypoint, Waypoint>();

         // init with d(start)=0
         distances.Add(start, 0);

         Waypoint current = start;
         visited.Add(start);

         while (true)
         {
            var unvisitedNeighbours = current.Connections.Where(c => !visited.Contains(c));
            UpdateDistances(current, unvisitedNeighbours, distances, pres);

            current = distances.Keys.Where(k => !visited.Contains(k)).OrderBy(d => distances[d]).FirstOrDefault();

            if (current == null)
            {
               break;
            }
            else if (current == end)
            {
               found = true;
               break;
            }

            visited.Add(current);
         }


         if (found)
         {
            Path p = new Path();

            current = end;

            while (current != start)
            {
               var next = pres[current];
               p.Waypoints.Insert(0, pres[current]);
               current = next;
            }

            return p;
         }

         return null;
      }

      private void UpdateDistances(Waypoint current, IEnumerable<Waypoint> neighbours, Dictionary<Waypoint, double> distances, Dictionary<Waypoint, Waypoint> pres)
      {
         var currentDist = GetDistance(distances, current);

         foreach (var neighbour in neighbours)
         {
            var neighbourDistance = GetDistance(distances, neighbour);
            var distToNeighbour = current.DistanceTo(neighbour);

            var distSum = currentDist + distToNeighbour;

            if (distSum < neighbourDistance)
            {
               SetDistance(distances, neighbour, distSum);
               SetPredecessor(pres, neighbour, current);
            }
         }
      }

      private void SetPredecessor(Dictionary<Waypoint, Waypoint> pres, Waypoint wp, Waypoint predecessor)
      {
         if (pres.ContainsKey(wp))
         {
            pres[wp] = predecessor;
         }
         else
         {
            pres.Add(wp, predecessor);
         }
      }

      private void SetDistance(Dictionary<Waypoint, double> distances, Waypoint wp, double distance)
      {
         if (distances.ContainsKey(wp))
         {
            distances[wp] = distance;
         }
         else
         {
            distances.Add(wp, distance);
         }
      }

      private double GetDistance(Dictionary<Waypoint, double> distances, Waypoint wp)
      {
         return distances.ContainsKey(wp) ? distances[wp] : double.MaxValue;
      }
   }
}
