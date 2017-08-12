using SteuerSoft.Maps.Core.Material;

namespace SteuerSoft.Maps.Controls.MonoGame.Markers.Interface
{
   public interface IIconMarker
   {
      /// <summary>
      /// The name of the texture. The texture shall be
      /// at the specified icon texture path. May be passed with or without file
      /// name extension.
      /// </summary>
      string TextureName { get; }

      /// <summary>
      /// The size of the output texture. May differ from the actual texture size.
      /// Drawing controls shall then scale it to the given size.
      /// </summary>
      MapSize Size { get; }

      /// <summary>
      /// An offset from the actual point where the marker is pointing to the top-left corner.
      /// This shall be given in ratios to make it independent from the actual texture size.
      /// This means, that if the markers point is on the bottom side of the texture in the middle,
      /// the offset shall be X = -0.5, Y = -1.0
      /// </summary>
      MapVectorD Offset { get; }

      /// <summary>
      /// The position of the marker in Geo-Coordinates.
      /// </summary>
      MapPointLatLon Position { get; }
   }
}
