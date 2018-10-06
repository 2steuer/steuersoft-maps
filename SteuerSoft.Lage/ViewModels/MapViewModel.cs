using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteuerSoft.Lage.ViewModels.Base;
using SteuerSoft.Maps.Controls.MonoGame.ValueTypes;
using SteuerSoft.Maps.Core.Material;

namespace SteuerSoft.Lage.ViewModels
{
    class MapViewModel : ViewModelBase
    {
        public string Title { get; set; } = "Map";
        public MapPointLatLon ViewPosition { get; set; } = new MapPointLatLon(0, 0);
        public bool CanMove { get; set; } = true;
        public ZoomingType ZoomMode { get; set; } = ZoomingType.Center;
        public bool CanZoom { get; set; } = true;
        public int Zoom { get; set; } = 1;
        public bool DrawCross { get; set; } = false;
        public bool DrawTileBorders { get; set; } = false;
    }
}
