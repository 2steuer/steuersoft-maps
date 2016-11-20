using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteuerSoft.Maps.Core.Interfaces;
using SteuerSoft.Maps.Core.Material;

namespace SteuerSoft.Maps.Projections
{
   /// <summary>
   /// This class represents the - in tiled maps - most commonly used Mercator Transformation.
   /// There are quite a lot descriptions out there how the calculation has to take place. See one on
   /// 
   /// http://wiki.openstreetmap.org/wiki/Mercator
   /// 
   /// The Mercator transformation transforms all coordinates from latitudes of -85.0511 to 85.0511 to -180 to 180.
   /// This results in the world being projected on a huge square which makes calculations quite easy.
   /// </summary>
   public class MercatorProjection : IProjection
   {
      /// <summary>
      /// The minimum input latitude.
      /// </summary>
      private static readonly double _minLat = -85.0511287798066;

      /// <summary>
      /// The maximum input latitude.
      /// </summary>
      private static readonly double _maxLat = 85.0511287798066;

      /// <summary>
      /// The minimum output latitude
      /// </summary>
      private static readonly double _minProjectedLat = -180;

      /// <summary>
      /// The maximum output latitude.
      /// </summary>
      private static readonly double _maxProjectedLat = 180;

      /// <summary>
      /// The minimum input longitude.
      /// </summary>
      private static readonly double _minLon = -180;

      /// <summary>
      /// The maximum input latitude.
      /// </summary>
      private static readonly double _maxLon = 180;

      /// <summary>
      /// The singleton instance of the projection class.
      /// </summary>
      private static MercatorProjection _instance = null;

      public double MinProjectedLon => _minLon;
      public double MaxProjectedLon => _maxLon;
      public double MinProjectedLat => _minProjectedLat;
      public double MaxProjectedLat => _maxProjectedLat;

      /// <summary>
      /// Initialises a new instance of the mercator projection.
      /// Private since we are usiong singleton pattern here.
      /// </summary>
      private MercatorProjection()
      {
         
      }

      /// <summary>
      /// Returns the singleton instance of the projection.
      /// If there has not been any instance by now, it creates one.
      /// </summary>
      /// <returns>The mercator projection instance.</returns>
      public static MercatorProjection GetInstance()
      {
         if (_instance == null)
         {
            _instance = new MercatorProjection();
         }

         return _instance;
      }

      public MapPointLatLon ToProjection(MapPointLatLon original)
      {
         MapPointLatLon newPoint = new MapPointLatLon();
         newPoint.Lon = Clip(original.Lon, _minLon, _maxLon);

         original.Lat = Clip(original.Lat, _minLat, _maxLat);

         newPoint.Lat = 180.0/Math.PI*
                        Math.Log(
                           Math.Tan(
                              Math.PI/4.0 + (original.Lat*(Math.PI/180.0))/2));



         return newPoint;
      }

      public MapPointLatLon FromProjection(MapPointLatLon projected)
      {
         MapPointLatLon newPoint = new MapPointLatLon();
         newPoint.Lon = Clip(projected.Lon, _minLon, _maxLon);

         projected.Lat = Clip(projected.Lat, _minProjectedLat, _maxProjectedLat);

         newPoint.Lat = 180.0/Math.PI*
                        (2*
                         Math.Atan(
                            Math.Exp(projected.Lat*Math.PI/180)) - Math.PI/2);

         

         return newPoint;
      }

      private double Clip(double val, double min, double max)
      {
         if (val < min)
         {
            val = min;
         }
         else if (val > max)
         {
            val = max;
         }

         return val;
      }
   }
}
