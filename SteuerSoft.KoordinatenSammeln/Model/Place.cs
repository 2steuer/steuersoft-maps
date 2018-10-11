using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteuerSoft.KoordinatenSammeln.Model
{
    class Place
    {
        public DateTime Time { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string UtmRef { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Task { get; set; }
    }
}
