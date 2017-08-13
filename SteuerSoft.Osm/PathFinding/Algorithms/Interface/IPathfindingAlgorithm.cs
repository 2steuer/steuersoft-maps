using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteuerSoft.Osm.StreetNetwork.Material;

namespace SteuerSoft.Osm.PathFinding.Algorithms.Interface
{
   public interface IPathfindingAlgorithm
   {
      Path FindPath(Waypoint start, Waypoint end);
   }
}
