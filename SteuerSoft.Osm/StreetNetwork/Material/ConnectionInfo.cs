using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteuerSoft.Osm.Material;
using SteuerSoft.Osm.StreetNetwork.Singleton;

namespace SteuerSoft.Osm.StreetNetwork.Material
{
    public class ConnectionInfo
    {
        public double MaxSpeed { get; set; }

        public Dictionary<string, string> Tags { get; } = new Dictionary<string, string>();

        public ConnectionInfo()
        {
            
        }

        public ConnectionInfo(Dictionary<string, string> tags)
        {
            Tags = tags;
            ParseTags();
        }

        public void ParseTags()
        {
            // 1. Parse MaxSpeed
            if (!Tags.ContainsKey("maxspeed"))
            {
                MaxSpeed = Defaults.DefaultMaxSpeeds[Tags["highway"]];
            }
            else
            {
                var speed = Tags["maxspeed"];

                if (speed == "none")
                {
                    MaxSpeed = Defaults.MaxSpeed;

                }
                else
                {
                    try
                    {
                        MaxSpeed = double.Parse(speed);
                    }
                    catch (Exception)
                    {
                        MaxSpeed = Defaults.DefaultMaxSpeeds[Tags["highway"]];
                    }

                }
            }
        }
    }
}
