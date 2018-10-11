using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NmeaParser;
using SteuerSoft.KoordinatenSammeln.Commands;
using SteuerSoft.KoordinatenSammeln.ViewModel.Base;

namespace SteuerSoft.KoordinatenSammeln.ViewModel
{
    class ConnectionViewModel : ViewModelBase
    {
        public event EventHandler<NmeaMessageReceivedEventArgs> Received;

        public ActionCommand RefreshPortsCommand { get; }
        public ActionCommand ConnectCommand { get; }

        public string ButtonText { get; set; } = "Connect";

        public string StatusText { get; set; } = "Not connected.";

        public string[] Ports { get; set; }
        public int[] BaudRates { get; set; }

        public string Port { get; set; } = "COM1";

        public int Baud { get; set; } = 9600;

        private SerialPort _port;
        private NmeaDevice _nmea;

        public ConnectionViewModel()
        {
            Ports = SerialPort.GetPortNames();
            BaudRates = new[] { 4800, 9600, 38400, 115200 };

            RefreshPortsCommand = new ActionCommand(RefreshPorts);
            ConnectCommand = new ActionCommand(ConnectDisconnect);
        }

        private void ConnectDisconnect()
        {
            if (_port != null && _port.IsOpen)
            {
                _port.Close();
                _port.Dispose();
                _nmea.Dispose();

                _port = null;
                _nmea = null;

                ButtonText = "Connect";
                StatusText = "Not Connected.";
            }
            else
            {
                try
                {
                    _port = new SerialPort(Port, Baud);
                    _port.Open();
                    _nmea = new StreamDevice(_port.BaseStream);
                    _nmea.OpenAsync().Wait();

                    _nmea.MessageReceived += (sender, args) => { Received?.Invoke(sender, args); };
                    ButtonText = "Disconnect";
                    StatusText = $"Connected to {Port}";
                }
                catch (Exception e)
                {
                    StatusText = $"Not connected: {e.GetType()}";
                    _port = null;
                    _nmea = null;
                }
            }
        }

        private void RefreshPorts()
        {
            Ports = SerialPort.GetPortNames();
        }
    }
}
