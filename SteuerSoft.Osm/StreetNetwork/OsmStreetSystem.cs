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
using SteuerSoft.Osm.StreetNetwork.Saving;
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

        public static OsmStreetSystem FromOsmFile(string file)
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

        public void SaveSystem(string file)
        {
            using (var writer = File.Open(file, FileMode.Create))
            {
                SaveSystem(writer);
            }
        }

        public void SaveSystem(Stream outStream)
        {
            Dictionary<ConnectionInfo, long> infos = new Dictionary<ConnectionInfo, long>();
            List<RelationInfo> relations = new List<RelationInfo>();

            XmlWriterSettings set = new XmlWriterSettings();
            set.Indent = true;

            using (var wrt = XmlWriter.Create(outStream, set))
            {
                wrt.WriteStartDocument();
                wrt.WriteStartElement("System");
                // Write all waypoints with their IDs and save the 
                // info and relation information
                foreach (var waypoint in Waypoints.Values)
                {
                    WriteWaypoint(wrt, waypoint);

                    foreach (var connection in waypoint.Connections)
                    {
                        var i = waypoint.GetInfoTo(connection);

                        if (!infos.ContainsKey(i))
                        {
                            infos.Add(i, infos.Count);
                        }

                        relations.Add(new RelationInfo()
                        {
                            Start = waypoint.Id,
                            End = connection.Id,
                            Info = infos[i]
                        });
                    }
                }

                // Write all connection infos
                foreach (var l in infos)
                {
                    WriteConnectionInfo(wrt, l.Key, l.Value);
                }

                // Write all relations
                foreach (var relation in relations)
                {
                    wrt.WriteStartElement("rel");
                    wrt.WriteAttributeString("start", relation.Start.ToString());
                    wrt.WriteAttributeString("end", relation.End.ToString());
                    wrt.WriteAttributeString("ci", relation.Info.ToString());
                    wrt.WriteEndElement();
                }

                wrt.WriteEndElement();
            }
        }

        public static OsmStreetSystem LoadSystem(string filename)
        {
            using (var str = File.Open(filename, FileMode.Open))
            {
                return LoadSystem(str);
            }
        }

        public static OsmStreetSystem LoadSystem(Stream str)
        {
            OsmStreetSystem newSys = new OsmStreetSystem();

            Dictionary<long, ConnectionInfo> infos = new Dictionary<long, ConnectionInfo>();

            using (var rd = XmlReader.Create(str))
            {
                while (rd.Read())
                {
                    if (rd.NodeType != XmlNodeType.Element)
                    {
                        continue;
                    }

                    switch (rd.Name)
                    {
                        case "wp":
                            var wp = ParseWaypoint(rd);
                            newSys.Waypoints.Add(wp.Id, wp);
                            break;

                        case "ci":
                            var id = ReadInfoId(rd);
                            infos.Add(id, ReadInfo(rd));
                            break;

                        case "rel":
                            long start = long.Parse(rd.GetAttribute("start"));
                            long end = long.Parse(rd.GetAttribute("end"));
                            long ci = long.Parse(rd.GetAttribute("ci"));

                            newSys.Waypoints[start].ConnectTo(newSys.Waypoints[end], infos[ci]);
                            break;
                    }
                }
            }

            return newSys;
        }

        private void WriteWaypoint(XmlWriter wrt, Waypoint wp)
        {
            wrt.WriteStartElement("wp");
            wrt.WriteAttributeString("id", wp.Id.ToString());
            wrt.WriteAttributeString("lat", wp.Lat.ToString(CultureInfo.InvariantCulture));
            wrt.WriteAttributeString("lon", wp.Lon.ToString(CultureInfo.InvariantCulture));
            wrt.WriteEndElement();
        }

        private void WriteConnectionInfo(XmlWriter wrt, ConnectionInfo i, long id)
        {
            wrt.WriteStartElement("ci");
            wrt.WriteAttributeString("id", id.ToString());

            foreach (var tag in i.Tags)
            {
                wrt.WriteStartElement("tag");
                wrt.WriteAttributeString("k", tag.Key);
                wrt.WriteAttributeString("v", tag.Value);
                wrt.WriteEndElement();
            }

            wrt.WriteEndElement();
        }

        private static long ReadInfoId(XmlReader rd)
        {
            return long.Parse(rd.GetAttribute("id"));
        }

        private static ConnectionInfo ReadInfo(XmlReader rd)
        {
            ConnectionInfo i = new ConnectionInfo();

            if (!rd.IsEmptyElement)
            {
                while (rd.Read() && rd.NodeType != XmlNodeType.EndElement)
                {
                    if (rd.Name == "tag")
                    {
                        i.Tags.Add(rd.GetAttribute("k"), rd.GetAttribute("v"));
                    }
                }
            }

            i.ParseTags();
            return i;
        }

        public Path FindPath(Waypoint start, Waypoint end)
        {
            return Pathfinder.FindPath(start, end);
        }


    }
}
