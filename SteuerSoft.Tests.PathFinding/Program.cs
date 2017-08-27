using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteuerSoft.Osm.Loading;
using SteuerSoft.Osm.PathFinding.Algorithms;
using SteuerSoft.Osm.StreetNetwork;
using SteuerSoft.Osm.StreetNetwork.Singleton;

namespace SteuerSoft.Tests.PathFinding
{
    class Program
    {
        private OsmStreetSystem _streets = null;

        Stopwatch _sw = new Stopwatch();

        static void Main(string[] args)
        {
            var p = new Program();
            p.Run();
        }

        private void Run()
        {
            Defaults.Load();

            MeasureTime(() =>
            {
                Console.WriteLine("Reading network file...");
                _streets = OsmStreetSystem.LoadSystem("K:\\OsmData\\schleswig-holstein-latest.sn");
                Console.WriteLine("Finished.");
            });

            GC.Collect();

            var start = _streets.Waypoints[259796142];
            var end = _streets.Waypoints[1876454449];

            Console.WriteLine();
            Console.WriteLine($"Trying to find path from {start.Id} to {end.Id}");
            Console.WriteLine($"Direct distance: {start.DistanceTo(end):##.000} km");

            Console.WriteLine();
            Console.WriteLine("Using Dijkstra's allgorithm.");
            var dijkstra = new DijkstraAlgorithm();
            var astar = new AStarAlgorithm();

            Console.WriteLine(" --------- DIJKSTRA ----------");

            for (int i = 0; i < 3; i++)
            {
                MeasureTime(() =>
                {
                    Console.WriteLine($"[{i}] Looking for a path in the graph....");
                    var p = dijkstra.FindPath(start, end);

                    Console.WriteLine($"Finished in {dijkstra.Steps} steps.");
                    Console.WriteLine($"Inspected {dijkstra.InspectedNodes}, visited {dijkstra.VisitedNodes}");

                    if (p != null)
                    {
                        Console.WriteLine($"Found a path. {p.Waypoints.Count} waypoints. {p.Length:##.000} km.");
                    }
                    else
                    {
                        Console.WriteLine("No path found.");
                    }
                });
            }

            Console.WriteLine(" --------- A-STAR ----------");
            for (int i = 0; i < 3; i++)
            {
                MeasureTime(() =>
                {
                    Console.WriteLine($"[{i}] Looking for a path in the graph....");
                    var p = astar.FindPath(start, end);

                    Console.WriteLine($"Finished in {astar.Steps} steps.");
                    Console.WriteLine($"Inspected {astar.InspectedNodes}, visited {astar.VisitedNodes}");

                    if (p != null)
                    {
                        Console.WriteLine($"Found a path. {p.Waypoints.Count} waypoints. {p.Length:##.000} km.");
                    }
                    else
                    {
                        Console.WriteLine("No path found.");
                    }
                });
            }

            Console.ReadLine();
        }

        private void MeasureTime(Action action)
        {
            _sw.Reset();
            _sw.Start();
            action();
            _sw.Stop();

            Console.WriteLine($"-- Elapsed time: {_sw.Elapsed}");
        }
    }
}
