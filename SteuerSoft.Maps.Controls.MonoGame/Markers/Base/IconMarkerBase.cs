using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using SteuerSoft.Maps.Controls.MonoGame.Markers.Interface;
using SteuerSoft.Maps.Controls.MonoGame.Properties;
using SteuerSoft.Maps.Core.Material;

namespace SteuerSoft.Maps.Controls.MonoGame.Markers.Base
{
    public abstract class IconMarkerBase : IMapMarker
    {
        private string _textureName;

        public string TextureIdentifier { get; }
        public MapSize Size { get; }
        public MapVectorD Offset { get; }
        public MapPointLatLon Position { get; set; }
        public bool CanDrag { get; set; }
        public bool Visible { get; set; }

        protected IconMarkerBase(string textureName)
        {
            _textureName = textureName;
            TextureIdentifier = $"resx-icons-{textureName}";
            using (var bmp = (Bitmap) Icons.ResourceManager.GetObject(textureName))
            {
                Size = new MapSize(bmp.Width, bmp.Height);
            }
            Offset = new MapVectorD(Size.Width / 2.0, Size.Height);
        }

        public Stream GetImage()
        {
            using (var bmp = (Bitmap)Icons.ResourceManager.GetObject(_textureName))
            {
                MemoryStream str = new MemoryStream();
                bmp?.Save(str, ImageFormat.Png);
                str.Position = 0;
                return str;
            }
        }
    }
}
