using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SteuerSoft.Lage.ViewModels;
using SteuerSoft.Lage.ViewModels.Base;
using SteuerSoft.Maps.Controls.MonoGame.ValueTypes;
using SteuerSoft.Maps.Core.Material;
using Xceed.Wpf.AvalonDock.Layout;

namespace SteuerSoft.Lage
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<ViewModelBase> _documents = new ObservableCollection<ViewModelBase>();


        public MainWindow()
        {
            InitializeComponent();
            _docking.DocumentsSource = _documents;
            Loaded += MainWindow_Loaded;
            _docking.DocumentClosed += _docking_DocumentClosed;
            _docking.Layout.ElementAdded += Layout_ElementAdded;
            
            
            
        }

        private void ActiveContent_Closed1(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Layout_ElementAdded(object sender, Xceed.Wpf.AvalonDock.Layout.LayoutElementEventArgs e)
        {
        }

        private void ActiveContent_Closed(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void _docking_DocumentClosed(object sender, Xceed.Wpf.AvalonDock.DocumentClosedEventArgs e)
        {
            _documents.Remove(e.Document.Content as ViewModelBase);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {
            LayoutAnchorable la = new LayoutAnchorable();
            la.Content = new TestToolViewModel()
            {
                ButtonText = "Testbutton!",
                Title = "Tool"
            };
            la.AddToLayout(_docking, AnchorableShowStrategy.Left);

            _documents.Add(new MapViewModel()
            {
                Title = "Reinfeld",
                ViewPosition = new MapPointLatLon(53.8367624, 10.4702885),
                CanMove = true,
                CanZoom =  true,
                DrawCross = true,
                Zoom = 12,
                DrawTileBorders = false,
                ZoomMode = ZoomingType.Center
            });
        }

        private void BreakItemClick(object sender, RoutedEventArgs e)
        {
            Debugger.Break();
        }
    }
}
