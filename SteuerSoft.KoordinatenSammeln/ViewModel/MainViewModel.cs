using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CsvHelper;
using CsvHelper.Configuration;
using GeoUtility.GeoSystem;
using NmeaParser.Nmea.Gps;
using SteuerSoft.KoordinatenSammeln.Commands;
using SteuerSoft.KoordinatenSammeln.Model;
using SteuerSoft.KoordinatenSammeln.ViewModel.Base;
using SteuerSoft.Maps.Core.Material;

namespace SteuerSoft.KoordinatenSammeln.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        public ActionCommand CloseCommand { get; }
        public ActionCommand SavePlaceCommand { get; }

        public ConnectionViewModel ConnectionViewModel { get; } = new ConnectionViewModel();

        public GpsData GpsData { get; set; } = new GpsData();
        public Place CurrentPlace { get; set; } = new Place();

        public MapPointLatLon MapLocation { get; set; }

        private bool _trackOnMap = true;

        public bool TrackMeOnMap
        {
            get { return _trackOnMap;}
            set
            {
                _trackOnMap = value;
                CanMoveMap = !value;
            }
        }
        public bool CanMoveMap { get; set; } = false;

        public bool SaveTrack { get; set; } = true;

        public string LatStr { get; set; }
        public string LonStr { get; set; }
        public string DateStr { get; set; }
        public string SpeedString { get; set; }
        public string FixString { get; set; }
        public string UtmrefString { get; set; }

        public MainViewModel()
        {
            CloseCommand = new ActionCommand(CloseCommandAction);
            SavePlaceCommand = new ActionCommand(SavePlace);

            ConnectionViewModel.Received += ConnectionViewModel_Received;
        }

        private void ConnectionViewModel_Received(object sender, NmeaParser.NmeaMessageReceivedEventArgs e)
        {
            switch (e.Message)
            {
                case Gprmc rmc:
                    GpsData.FixInfo = FixString = rmc.Active ? "Fix" : "No Fix";
                    if (rmc.Active)
                    {
                        GpsData.Lat = rmc.Latitude;
                        LatStr = $"{rmc.Latitude:0.0000}";

                        GpsData.Lon = rmc.Longitude;
                        LonStr = $"{rmc.Longitude:0.0000}";

                        var dto = new DateTime(rmc.FixTime.Ticks, DateTimeKind.Utc);
                        
                        GpsData.Time = dto.ToLocalTime();
                        DateStr = $"{GpsData.Time:dd.MM.yyyy HH:mm:ss}";

                        GpsData.Speed = rmc.Speed * 1.852;

                        SpeedString = $"{GpsData.Speed:0.0} km/h";

                        var latlon = new Geographic(rmc.Longitude, rmc.Latitude);

                        GpsData.UtmRef = UtmrefString = ((MGRS)latlon).ToString();



                        if (TrackMeOnMap)
                        {
                            MapLocation = new MapPointLatLon(rmc.Latitude, rmc.Longitude);
                        }

                        if (SaveTrack)
                        {
                            try
                            {
                                using (var fs = File.AppendText("track.csv"))
                                using (var csv = new CsvWriter(fs, new Configuration() { Delimiter = ";" }))
                                {
                                    csv.WriteRecord(GpsData);
                                    csv.NextRecord();
                                    
                                }
                            }
                            catch (Exception exception)
                            {
                                SaveTrack = false;
                                ConnectionViewModel.StatusText = $"Failed to write track. {exception.Message}";
                            }
                            
                        }
                    }
                    break;

                
            }
        }

        private void CloseCommandAction()
        {
            Environment.Exit(0);
        }

        private void SavePlace()
        {
            CurrentPlace.Lat = GpsData.Lat;
            CurrentPlace.Lon = GpsData.Lon;
            CurrentPlace.UtmRef = GpsData.UtmRef;
            CurrentPlace.Time = DateTime.Now;



            try
            {
                using (var fs = File.AppendText("places.csv"))
                using (var csv = new CsvWriter(fs, new Configuration() { Delimiter = ";" }))
                {
                    csv.WriteRecord(CurrentPlace);
                    csv.NextRecord();
                }

                CurrentPlace = new Place();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
            

            
        }
    }
}
