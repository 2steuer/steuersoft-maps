using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SteuerSoft.Maps.Controls.MonoGame;
using SteuerSoft.Maps.Controls.MonoGame.ValueTypes;
using SteuerSoft.Maps.Core.Interfaces;
using SteuerSoft.Maps.Core.Material;
using Color = System.Windows.Media.Color;

namespace SteuerSoft.Maps.Wpf
{
    /// <summary>
    /// Interaktionslogik für Map.xaml
    /// </summary>
    public partial class Map : UserControl
    {
        public static readonly DependencyProperty ProviderProperty = DependencyProperty.Register(
            "Provider", typeof(IMapProvider), typeof(Map), new PropertyMetadata(default(IMapProvider), ProviderChanged));

        public IMapProvider Provider
        {
            get { return (IMapProvider) GetValue(ProviderProperty); }
            set { SetValue(ProviderProperty, value); }
        }

        public static readonly DependencyProperty MapPositionProperty = DependencyProperty.Register(
            "MapPosition", typeof(MapPointLatLon), typeof(Map), new PropertyMetadata(default(MapPointLatLon), PositionChangedEvent));

        public MapPointLatLon MapPosition
        {
            get { return (MapPointLatLon)GetValue(MapPositionProperty); }
            set { SetValue(MapPositionProperty, value); }
        }

        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(
            "Zoom", typeof(int), typeof(Map), new PropertyMetadata(1, ZoomChangedEvent));

        public int Zoom
        {
            get => (int)GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, value);
        }

        public static readonly DependencyProperty ZoomingTypeProperty = DependencyProperty.Register(
            "ZoomingType", typeof(ZoomingType), typeof(Map), new PropertyMetadata(ZoomingType.Center, ZoomingTypeChangedEvent));

        public ZoomingType ZoomingType
        {
            get { return (ZoomingType)GetValue(ZoomingTypeProperty); }
            set { SetValue(ZoomingTypeProperty, value); }
        }

        public static readonly DependencyProperty MiddleCrossProperty = DependencyProperty.Register(
            "MiddleCross", typeof(bool), typeof(Map), new PropertyMetadata(true, PropertyChangedCallback));

        public bool MiddleCross
        {
            get { return (bool) GetValue(MiddleCrossProperty); }
            set { SetValue(MiddleCrossProperty, value); }
        }

        public static readonly DependencyProperty TileBordersProperty = DependencyProperty.Register(
            "TileBorders", typeof(bool), typeof(Map), new PropertyMetadata(false, TileBordersChangedCallbackl ));

        public bool TileBorders
        {
            get { return (bool) GetValue(TileBordersProperty); }
            set { SetValue(TileBordersProperty, value); }
        }

        public static readonly DependencyProperty CanMoveProperty = DependencyProperty.Register(
            "CanMove", typeof(bool), typeof(Map), new PropertyMetadata(true, CanMoveChangedCallback));

        public static readonly DependencyProperty CanZoomProperty = DependencyProperty.Register(
            "CanZoom", typeof(bool), typeof(Map), new PropertyMetadata(true, CanZoomChangedCallback));

        public bool CanZoom
        {
            get { return (bool) GetValue(CanZoomProperty); }
            set { SetValue(CanZoomProperty, value); }
        }

        public bool CanMove
        {
            get { return (bool) GetValue(CanMoveProperty); }
            set { SetValue(CanMoveProperty, value); }
        }

        public Map()
        {
            InitializeComponent();

            
        }

        private static void PositionChangedEvent(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var m = (dependencyObject as Map).Scene.Map;

            if (m != null)
            {
                m.Position = (MapPointLatLon)e.NewValue;
            }
        }

        private static void ZoomChangedEvent(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var m = (dependencyObject as Map).Scene.Map;

            if (m != null)
            {
                m.Zoom = (int)e.NewValue;
            }
        }

        private static void ZoomingTypeChangedEvent(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var m = (dependencyObject as Map).Scene.Map;
            if (m != null)
            {
                m.ZoomMode =
                    (ZoomingType)e.NewValue;

            }
        }

        private static void PropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var m = (dependencyObject as Map).Scene.Map;
            if (m != null)
            {
                m.DrawMiddleCross = (bool)dependencyPropertyChangedEventArgs.NewValue;
            }
        }

        private static void TileBordersChangedCallbackl(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var m = (dependencyObject as Map).Scene.Map;
            if (m != null)
            {
                m.DrawTileBorders = (bool)dependencyPropertyChangedEventArgs.NewValue;
            }
        }

        private static void CanZoomChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var m = (dependencyObject as Map).Scene.Map;
            if (m != null)
            {
                m.CanZoom = (bool)dependencyPropertyChangedEventArgs.NewValue;
            }
        }


        private static void CanMoveChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var m = (dependencyObject as Map).Scene.Map;
            if (m != null)
            {
                m.CanMove = (bool)dependencyPropertyChangedEventArgs.NewValue;
            }
        }

        private static void ProviderChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var m = (dependencyObject as Map).Scene.Map;
            if (m != null)
            {
                m.Provider = (IMapProvider)dependencyPropertyChangedEventArgs.NewValue;
            }
        }



        private void Scene_OnInitialized(object sender)
        {
            MapControl map = Scene.Map;
            map.Position = MapPosition;
            map.ZoomMode = ZoomingType;
            map.Zoom = Zoom;
            map.DrawMiddleCross = MiddleCross;
            map.DrawTileBorders = TileBorders;
            map.CanMove = CanMove;
            map.CanZoom = CanZoom;
            map.Provider = Provider;

            map.OnZoomed += Map_OnZoomed;
            map.OnMoved += Map_OnMoved;
        }

        private void Map_OnMoved(object sender, MapPointLatLon newPosition)
        {
            MapPosition = newPosition;
        }

        private void Map_OnZoomed(object sender, int newZoom)
        {
            Zoom = newZoom;
        }
    }
}
