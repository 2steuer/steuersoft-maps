using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteuerSoft.Maps.Core.Material.Elements.Path;

namespace SteuerSoft.Maps.Core.Material.Elements.Polygon
{
   public class MapPolygon : MapPath
   {
      public MapPolygon(IEnumerable<MapPointLatLon> points)
         : base(points)
      {
         
      }
   }
}
