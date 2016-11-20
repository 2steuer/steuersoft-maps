using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteuerSoft.Maps.Core.Material
{
   /// <summary>
   /// This struct represents a Point containing out of a longitude and a latitude.
   /// Usually stored geo coordinates or projected geo coordinates.
   /// </summary>
   public struct MapPointLatLon
   {
      /// <summary>
      /// The Latitude value in degrees.
      /// </summary>
      public double Lat { get; set; }

      /// <summary>
      /// The Longitude value in degrees.
      /// </summary>
      public double Lon { get; set; }

      /// <summary>
      /// Initialises the new Lat/Lon Point with values.
      /// </summary>
      /// <param name="lat">Latitude</param>
      /// <param name="lon">Longitude</param>
      public MapPointLatLon(double lat, double lon)
      {
         Lat = lat;
         Lon = lon;
      }
   }
}
