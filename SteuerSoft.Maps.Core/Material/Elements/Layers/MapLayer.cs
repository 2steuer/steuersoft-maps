using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteuerSoft.Maps.Core.Material.Elements.Path;
using SteuerSoft.Maps.Core.Material.Elements.Polygon;

namespace SteuerSoft.Maps.Core.Material.Elements.Layers
{
    public class MapLayer
    {
        public string Name { get; set; }
        public bool Visible { get; set; }

        public List<MapPath> Paths { get; } = new List<MapPath>();
        public List<MapPolygon> Polygons { get; } = new List<MapPolygon>();
    }
}
