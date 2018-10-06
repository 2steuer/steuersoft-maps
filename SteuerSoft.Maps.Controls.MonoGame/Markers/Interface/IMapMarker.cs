using System.IO;
using Microsoft.Xna.Framework;
using SteuerSoft.Maps.Core.Material;

namespace SteuerSoft.Maps.Controls.MonoGame.Markers.Interface
{
    public interface IMapMarker
    {
        /// <summary>
        /// Texture identifier for the icon texture. A texture identifier shall be globally
        /// unique for every image displayed. When the image of the marker changes, the texture identifier
        /// also must change. When two markers have the same image, they shall show the same identifier.
        /// 
        /// This identifier is used for caching textures in the graphics memory, as with a lot of icons
        /// on the map the memory may be used a lot if every icon loads the same image again.
        /// </summary>
        string TextureIdentifier { get; }

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
        MapPointLatLon Position { get; set; }

        /// <summary>
        /// Indicates that the user can move the marker with the mouse.
        /// </summary>
        bool CanDrag { get; set; }

        /// <summary>
        /// Flag indicating whether the marker shall be rendered.
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Returns a memory stream with the image that
        /// can be parsed by Texture2D.FromStream().
        /// 
        /// This function is called whenever a new <see cref="TextureIdentifier"/> is found.
        /// </summary>
        /// <returns>A MemoryStream containing the image data of the marker image</returns>
        Stream GetImage();
    }
}
