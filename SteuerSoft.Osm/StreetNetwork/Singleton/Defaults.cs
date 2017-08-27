using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SteuerSoft.Osm.StreetNetwork.Singleton
{
    public static class Defaults
    {
        public static HashSet<string> HighwayWhitelist { get; } = new HashSet<string>();

        public static Dictionary<string, double> DefaultMaxSpeeds { get; } = new Dictionary<string, double>();

        public static double MaxSpeed { get; set; } = 130;

        public static void Load(string file = null)
        {
            if (file == null)
            {
                file = "NetworkConfig.xml";
            }

            var doc = XDocument.Load(file);

            var speedEle = doc.Root.Element("MaxSpeed");
            MaxSpeed = double.Parse(speedEle.Attribute("value").Value);

            var filterEle = doc.Root.Element("HighwayFilter");

            foreach (var element in filterEle.Elements("Entry"))
            {
                string name = element.Attribute("type").Value;
                double speed = double.Parse(element.Attribute("maxspeed").Value);

                HighwayWhitelist.Add(name);
                DefaultMaxSpeeds.Add(name, speed);
            }
        }
    }
}
