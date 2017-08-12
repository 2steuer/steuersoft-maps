using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteuerSoft.Maps.Core.Material.Elements.Path
{
   public class MapPath
   {
      public double LineWidth { get; set; } = 3;

      public List<MapPointLatLon> Points { get; } = new List<MapPointLatLon>();

      public MapPath(IEnumerable<MapPointLatLon> points)
      {
         Points.AddRange(points);
      }
   }
}
