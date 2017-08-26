using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Priority_Queue;
using SteuerSoft.Osm.PathFinding.Algorithms.Interface;
using SteuerSoft.Osm.StreetNetwork.Material;

namespace SteuerSoft.Osm.PathFinding.Algorithms
{
   public class DijkstraAlgorithm : IPathfindingAlgorithm
   {
      public int Steps = 0;
      public int VisitedNodes = 0;
      public int InspectedNodes = 0;

      private Dictionary<Waypoint, double> _distances = new Dictionary<Waypoint, double>();
      SimplePriorityQueue<Waypoint, double> _distancesSorted = new SimplePriorityQueue<Waypoint, double>();
      HashSet<Waypoint> _visited = new HashSet<Waypoint>();
      Dictionary<Waypoint, Waypoint> _predecessors = new Dictionary<Waypoint, Waypoint>();

      public Func<Waypoint, Waypoint, double> CostFunction { get; set; }

      public DijkstraAlgorithm()
         :this((wpstart, wpend) => wpstart.DistanceTo(wpend)/wpstart.GetInfoTo(wpend).MaxSpeed)
      {
         
      }

      public DijkstraAlgorithm(Func<Waypoint, Waypoint, double> costFunc)
      {
         CostFunction = costFunc;
      }

      public Path FindPath(Waypoint start, Waypoint end)
      {
         bool found = false;

         _distances.Clear();
         _distancesSorted.Clear();
         _visited.Clear();
         _predecessors.Clear();

         // init with d(start)=0
         _distances.Add(start, 0);

         Waypoint current = start;
         _visited.Add(start);

         Steps = 0;
         VisitedNodes = 0;
         InspectedNodes = 0;

         while (true)
         {
            Steps++;
            var unvisitedNeighbours = current.Connections.Where(c => !_visited.Contains(c));
            UpdateDistances(current, unvisitedNeighbours);


            current = _distancesSorted.Count > 0 ? _distancesSorted.Dequeue() : null;

            if (current == null)
            {
               break;
            }
            else if (current == end)
            {
               
               found = true;
               break;
            }
            
            _visited.Add(current);
         }

         VisitedNodes = _visited.Count;
         InspectedNodes = _distances.Count;

         if (found)
         {
            Path p = new Path();

            current = end;

            while (current != start)
            {
               var next = _predecessors[current];
               p.Waypoints.Insert(0, _predecessors[current]);
               current = next;
            }

            return p;
         }

         return null;
      }

      private void UpdateDistances(Waypoint current, IEnumerable<Waypoint> neighbours)
      {
         double currentDist = GetDistance(current);
         foreach (var neighbour in neighbours)
         {
            var neighbourDistance = GetDistance(neighbour);
            var distToNeighbour = CostFunction(current, neighbour);

            var distSum = currentDist + distToNeighbour;

            if (distSum < neighbourDistance)
            {
               SetDistance(neighbour, distSum);
               SetPredecessor(neighbour, current);
            }
         }
      }

      private void SetPredecessor(Waypoint wp, Waypoint predecessor)
      {
         if (_predecessors.ContainsKey(wp))
         {
            _predecessors[wp] = predecessor;
         }
         else
         {
            _predecessors.Add(wp, predecessor);
         }
      }

      private void SetDistance(Waypoint wp, double distance)
      {
         if (_distances.ContainsKey(wp))
         {
            _distances[wp] = distance;
            _distancesSorted.UpdatePriority(wp, distance);
         }
         else
         {
            _distances.Add(wp, distance);
            _distancesSorted.Enqueue(wp, distance);
         }
      }

      private double GetDistance(Waypoint wp)
      {
         return _distances.ContainsKey(wp) ? _distances[wp] : double.MaxValue;
      }
   }
}
