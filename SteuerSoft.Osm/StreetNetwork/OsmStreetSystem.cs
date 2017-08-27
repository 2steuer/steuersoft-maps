using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using SteuerSoft.Osm.Extensions;
using SteuerSoft.Osm.Loading;
using SteuerSoft.Osm.Material;
using SteuerSoft.Osm.PathFinding.Algorithms;
using SteuerSoft.Osm.PathFinding.Algorithms.Interface;
using SteuerSoft.Osm.StreetNetwork.Material;
using SteuerSoft.Osm.StreetNetwork.Singleton;
using Path = SteuerSoft.Osm.StreetNetwork.Material.Path;

namespace SteuerSoft.Osm.StreetNetwork
{
    public class OsmStreetSystem
    {

        public Dictionary<long, Waypoint> Waypoints { get; } = new Dictionary<long, Waypoint>();

        public IPathfindingAlgorithm Pathfinder { get; set; } = new DijkstraAlgorithm();

        private OsmStreetSystem()
        {

        }

        private Waypoint GetWaypointForNode(OsmNode node)
        {
            if (Waypoints.ContainsKey(node.Id))
            {
                return Waypoints[node.Id];
            }
            else
            {
                Waypoint nw = new Waypoint(node);
                Waypoints.Add(node.Id, nw);
                return nw;
            }
        }

        public static OsmStreetSystem Build(OsmLoader data)
        {
            OsmStreetSystem newSystem = new OsmStreetSystem();

            var ways = data.Ways.Values.Where(w => Defaults.HighwayWhitelist.Contains(w.GetTag("highway", "-")));

            foreach (var way in ways)
            {
                bool oneWay = (way.GetTag("oneway", "no") == "yes")
                   || (way.GetTag("junction", "-") == "roundabout");

                Waypoint lastPoint = newSystem.GetWaypointForNode(way.Nodes[0]);

                var conInfo = MakeConnectionInfo(way);

                for (int i = 1; i < way.Nodes.Count; i++)
                {
                    var current = newSystem.GetWaypointForNode(way.Nodes[i]);

                    lastPoint.ConnectTo(current, conInfo);

                    if (!oneWay)
                    {
                        current.ConnectTo(lastPoint, conInfo);
                    }

                    lastPoint = current;
                }
            }

            return newSystem;
        }

        private static ConnectionInfo MakeConnectionInfo(Dictionary<string, string> tags)
        {
            return new ConnectionInfo(tags);
        }

        private static ConnectionInfo MakeConnectionInfo(OsmWay way)
            => MakeConnectionInfo(way.Tags);

        public static OsmStreetSystem FromFile(string file)
        {
            Dictionary<long, Waypoint> wps = new Dictionary<long, Waypoint>();
            OsmStreetSystem newSystem = new OsmStreetSystem();

            using (XmlReader rd = XmlReader.Create(file))
            {
                while (rd.Read())
                {
                    if (rd.NodeType == XmlNodeType.Element)
                    {
                        switch (rd.Name)
                        {
                            case "node":
                                var newwp = ParseWaypoint(rd);
                                wps.Add(newwp.Id, newwp);
                                break;

                            case "way":
                                ParseWay(rd, wps, newSystem);
                                break;
                        }
                    }
                }
            }

            return newSystem;
        }

        private static Waypoint ParseWaypoint(XmlReader rd)
        {
            Waypoint newWp = new Waypoint(
                long.Parse(rd.GetAttribute("id")),
                double.Parse(rd.GetAttribute("lat"), CultureInfo.InvariantCulture),
                double.Parse(rd.GetAttribute("lon"), CultureInfo.InvariantCulture));

            return newWp;

        }

        private static void ParseWay(XmlReader rd, Dictionary<long, Waypoint> wps, OsmStreetSystem strt)
        {
            if (rd.IsEmptyElement)
                return;

            var wpIds = new ArrayList(2000);
            Dictionary<string, string> tags = new Dictionary<string, string>();

            while (rd.Read() && rd.NodeType != XmlNodeType.EndElement)
            {
                if (rd.NodeType == XmlNodeType.Element)
                {
                    switch (rd.Name)
                    {
                        case "nd":
                            wpIds.Add(long.Parse(rd.GetAttribute("ref")));
                            break;

                        case "tag":
                            tags.Add(
                                    rd.GetAttribute("k"),
                                    rd.GetAttribute("v")
                                );
                            break;
                    }
                }
            }

            if (!Defaults.HighwayWhitelist.Contains(tags.Get("highway", "-")))
            {
                return;
            }

            bool oneWay = (tags.Get("oneway", "no") == "yes")
                   || (tags.Get("junction", "-") == "roundabout");

            var info = MakeConnectionInfo(tags);

            Waypoint last = wps[(long)wpIds[0]];

            if (!strt.Waypoints.ContainsKey(last.Id))
            {
                strt.Waypoints.Add(last.Id, last);
            }

            for (int i = 1; i < wpIds.Count; i++)
            {
                Waypoint current = wps[(long)wpIds[i]];
                if (!strt.Waypoints.ContainsKey(current.Id))
                {
                    strt.Waypoints.Add(current.Id, current);
                }

                last.ConnectTo(current, info);

                if (!oneWay)
                {
                    current.ConnectTo(last, info);
                }

                last = current;
            }
        }

        public Path FindPath(Waypoint start, Waypoint end)
        {
            return Pathfinder.FindPath(start, end);
        }


    }
}
