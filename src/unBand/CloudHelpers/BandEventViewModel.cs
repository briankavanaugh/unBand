using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using unBand.Cloud;
using unBand.Cloud.Events;

namespace unBand.CloudHelpers
{
    public class BandEventViewModel : INotifyPropertyChanged
    {
        private readonly BandCloudClient _cloud;
        private bool _hasGPSPoints;
        private bool _loaded;

        public BandEventViewModel(BandCloudClient cloud, BandEventBase cloudEvent)
        {
            _cloud = cloud;

            Event = cloudEvent;

            if (Event is UserDailyActivity)
            {
                // this event type is considered "Loaded" already since we get all of the information
                // from the initial API call
                Loaded = true;
            }
        }

        public BandEventBase Event { get; private set; }

        public bool HasGPSPoints
        {
            get { return _hasGPSPoints; }
            set
            {
                if (_hasGPSPoints != value)
                {
                    _hasGPSPoints = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool Loaded
        {
            get { return _loaded; }
            set
            {
                if (_loaded != value)
                {
                    _loaded = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public async Task LoadFull()
        {
            if (!Loaded)
            {
                await Task.Run(async () =>
                {
                    var fullData = await _cloud.GetFullEventData(Event.EventID, Event.Expanders);

                    Event.InitFullEventData(fullData);

                    Loaded = true;

                    HasGPSPoints = (Event is IBandEventWithMapPoints) && ((IBandEventWithMapPoints) Event).HasGPSPoints;
                });
            }
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