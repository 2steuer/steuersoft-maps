using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteuerSoft.Maps.Core.Material;
using SteuerSoft.Osm.Material;

namespace SteuerSoft.Maps.MonoGame.OsmExtensions
{
   static class OsmExtensions
   {
      public static MapPointLatLon ToMapPointLatLon(this OsmNode node)
      {
         return new MapPointLatLon(node.Lat, node.Lon);
      }
   }
}
