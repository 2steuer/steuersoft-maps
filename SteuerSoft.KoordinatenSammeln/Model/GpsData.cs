using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SteuerSoft.KoordinatenSammeln.Model
{
    class GpsData : INotifyPropertyChanged
    {
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string UtmRef { get; set; }
        public DateTime Time { get; set; }
        public double Speed { get; set; }
        public string FixInfo { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
