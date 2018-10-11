using SteuerSoft.KoordinatenSammeln.ViewModel.Base;
using SteuerSoft.Maps.Controls.MonoGame.ValueTypes;
using SteuerSoft.Maps.Core.Interfaces;
using SteuerSoft.Maps.Core.Material;
using SteuerSoft.Maps.Providers;

namespace SteuerSoft.KoordinatenSammeln.ViewModel
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
        public IMapProvider Provider { get; set; } = GermanOsmProvider.GetInstance();
    }
}
