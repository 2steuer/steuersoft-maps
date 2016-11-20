using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteuerSoft.Maps.Core.Material;

namespace SteuerSoft.Maps.Core.Interfaces
{
   /// <summary>
   /// Classes inheriting from this interface represent a specific projection of the Geo-Coordinates to some other coordinate system.
   /// </summary>
   public interface IProjection
   {
      /// <summary>
      /// The minimum longitude that is possible to be put out of this projection.
      /// </summary>
      double MinProjectedLon { get; }

      /// <summary>
      /// The maximum longitude that is possible to be put out of this projection.
      /// </summary>
      double MaxProjectedLon { get; }

      /// <summary>
      /// The minimum latitude that is possible to be put out of this projection.      
      /// </summary>
      double MinProjectedLat { get; }

      /// <summary>
      /// The maximum latitude that is possible to be put out of this projection.      
      /// </summary>
      double MaxProjectedLat { get; }

      /// <summary>
      /// Transforms a given point out of Latitude/Longitude Geo-Coordinates to a point
      /// within the projection system.
      /// </summary>
      /// <param name="original">The Lat/Lon Geocoordinate point.</param>
      /// <returns>The corresponding point within the projection coordinate system.</returns>
      MapPointLatLon ToProjection(MapPointLatLon original);

      /// <summary>
      /// Transforms a point from the projection system to a point in the Lat/Lon GeoCoordinates.
      /// </summary>
      /// <param name="projected">The point in the projection system.</param>
      /// <returns>The corresponding point in the Lat/Lon GeoCoordinate system.</returns>
      MapPointLatLon FromProjection(MapPointLatLon projected);
   }
}
