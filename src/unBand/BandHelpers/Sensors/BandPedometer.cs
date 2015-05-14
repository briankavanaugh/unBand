using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Band.Admin;
using Microsoft.Band.Admin.Streaming;

namespace unBand.BandHelpers.Sensors
{
    public class BandPedometer : INotifyPropertyChanged
    {
        private readonly ICargoClient _client;
        private uint _totalMovements;
        private uint _totalSteps;

        public BandPedometer(ICargoClient client)
        {
            _client = client;
        }

        public uint TotalSteps
        {
            get { return _totalSteps; }
            set
            {
                if (_totalSteps != value)
                {
                    _totalSteps = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public uint TotalMovements
        {
            get { return _totalMovements; }
            set
            {
                if (_totalMovements != value)
                {
                    _totalMovements = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public async Task InitAsync()
        {
            await OneTimePedometerReading();
        }

        /// <summary>
        ///     To start us off we get an initial, one-off, Pedometer reading.
        ///     To get consistent updates use TODO: StartPedometer();
        /// </summary>
        private async Task OneTimePedometerReading()
        {
            _client.PedometerUpdated += _client_OneTimePedometerUpdated;
            await _client.SensorSubscribeAsync(SensorType.Pedometer);
        }

        private void _client_OneTimePedometerUpdated(object sender, PedometerUpdatedEventArgs e)
        {
            _client.SensorUnsubscribe(SensorType.Pedometer);

            TotalSteps = e.TotalSteps;
            TotalMovements = e.TotalMovements;

            _client.PedometerUpdated -= _client_OneTimePedometerUpdated;
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if (PropertyChanged != null)
            {
                Application.Current.Dispatcher.BeginInvoke(
                    new Action(() => { PropertyChanged(this, new PropertyChangedEventArgs(propertyName)); }));
            }
        }

        #endregion
    }
}