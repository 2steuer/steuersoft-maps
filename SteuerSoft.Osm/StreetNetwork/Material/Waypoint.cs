using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteuerSoft.Osm.Material;

namespace SteuerSoft.Osm.StreetNetwork.Material
{
   public class Waypoint
   {
      OsmNode _node;

      public long Id => _node.Id;
      public double Lat => _node.Lat;
      public double Lon => _node.Lon;

      private readonly HashSet<Waypoint> _connections = new HashSet<Waypoint>();

      public IEnumerable<Waypoint> Connections => _connections;

      public Waypoint(OsmNode node)
      {
         _node = node;
      }

      public void ConnectTo(Waypoint wp)
      {
         _connections.Add(wp);
      }

      /// <summary>
      /// Gets the distance to another waypoint in meters.
      /// 
      /// Source:
      /// https://www.codeproject.com/Articles/12269/Distance-between-locations-using-latitude-and-long
      /// </summary>
      /// <param name="other"></param>
      /// <returns>Distance to the other point in meters.</returns>
      public double DistanceTo(Waypoint other)
      {
         double dDistance = Double.MinValue;
         double dLat1InRad = Lat * (Math.PI / 180.0);
         double dLong1InRad = Lon * (Math.PI / 180.0);
         double dLat2InRad = other.Lat * (Math.PI / 180.0);
         double dLong2InRad = other.Lon * (Math.PI / 180.0);

         double dLongitude = dLong2InRad - dLong1InRad;
         double dLatitude = dLat2InRad - dLat1InRad;

         // Intermediate result a.
         double a = Math.Pow(Math.Sin(dLatitude / 2.0), 2.0) +
                    Math.Cos(dLat1InRad) * Math.Cos(dLat2InRad) *
                    Math.Pow(Math.Sin(dLongitude / 2.0), 2.0);

         // Intermediate result c (great circle distance in Radians).
         double c = 2.0 * Math.Asin(Math.Sqrt(a));

         // Distance.
         // const Double kEarthRadiusMiles = 3956.0;
         const Double kEarthRadiusKms = 6376.5;
         dDistance = kEarthRadiusKms * c;

         return dDistance;

      }
   }
}
