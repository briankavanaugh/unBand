using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Band;
using Microsoft.Band.Admin;
using unBand.Cloud;

namespace unBand.BandHelpers
{
    internal class BandManager : INotifyPropertyChanged
    {
        private static DispatcherTimer _timer;
        private ICargoClient _cargoClient;
        private IBandInfo _deviceInfo;
        private bool _isConnected;
        private bool _isDesktopSyncAppRunning;
        private BandProperties _properties;
        private BandSensors _sensors;
        private BandTheme _theme;
        private BandTiles _tiles;

        private BandManager()
        {
        }

        #region Singleton

        /// <summary>
        ///     Call BandManager.Start() to kick things off htere
        ///     TODO: Consider an exception if Start() is not called
        /// </summary>
        public static BandManager Instance { get; private set; }

        #endregion

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                if (_isConnected != value)
                {
                    _isConnected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsDesktopSyncAppRunning
        {
            get { return _isDesktopSyncAppRunning; }
            set
            {
                if (_isDesktopSyncAppRunning != value)
                {
                    _isDesktopSyncAppRunning = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICargoClient CargoClient
        {
            get { return _cargoClient; }
            set
            {
                if (_cargoClient != value)
                {
                    _cargoClient = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public BandProperties Properties
        {
            get { return _properties; }
            set
            {
                if (_properties != value)
                {
                    _properties = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public BandTheme Theme
        {
            get { return _theme; }
            set
            {
                if (_theme != value)
                {
                    _theme = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public BandSensors Sensors
        {
            get { return _sensors; }
            set
            {
                if (_sensors != value)
                {
                    _sensors = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public BandTiles Tiles
        {
            get { return _tiles; }
            set
            {
                if (_tiles != value)
                {
                    _tiles = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public BandLogger Log
        {
            get { return BandLogger.Instance; }
        }

        public static bool CanRun(ref string message)
        {
            string msg = null;

            if (!CargoDll.BandDllsExist(ref msg))
            {
                message =
                    "Couldn't find the latest Microsoft Band Desktop Sync app.\n\nInstall it from: http://bit.ly/desktopband and try again.\n\nSadly we're going to have to exit now.\n\nDiagnostiscs: " +
                    msg;

                return false;
            }

            return true;
        }

        public static void Create()
        {
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            Instance = new BandManager();

            Instance.InitializeCargoLogging();
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("Microsoft.Band"))
            {
                // an exception will be thrown on Microsoft.Band.Desktop.resources, ignore it
                try
                {
                    var curDir = Directory.GetCurrentDirectory();
                    Directory.SetCurrentDirectory(CargoDll.GetOfficialBandDllPath());

                    var asm =
                        Assembly.LoadFrom(CargoDll.GetUnBandBandDll(args.Name.Substring(0, args.Name.IndexOf(','))));

                    Directory.SetCurrentDirectory(curDir);

                    return asm;
                }
                catch
                {
                    // TODO: log exception?
                }
            }

            return null;
        }

        public static void Start()
        {
            if (Instance == null)
                Create();

            // While I don't love the whole Interval = 1ms thing, it simply means that we trigger the first run immediately
            // TODO: Since this could potentially cause race conditions if someone really wanted to garauntee order
            //       of execution we could split out object creation from Start().
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1);

            _timer.Tick += async (sender, e) =>
            {
                _timer.IsEnabled = false;

                if (!Instance.IsConnected)
                {
                    await Instance.ConnectDevice();
                }
                else
                {
                    // make sure we still have the current device
                    var devices = await GetConnectedDevicesAsync();

                    if (!devices.Any(i => i.Name == Instance._deviceInfo.Name))
                    {
                        Instance.IsConnected = false;
                    }
                }

                // only reset once we finished processing
                _timer.Interval = TimeSpan.FromSeconds(10);
                _timer.IsEnabled = true;
            };

            _timer.Start();
        }

        private async Task ConnectDevice()
        {
            if (DesktopSyncAppIsRunning())
                return;

            var device = await GetUsbBand();

            if (device != null)
            {
                // since this will trigger binding, invoke on the UI thread
                Application.Current.Dispatcher.BeginInvoke(new Action(async () =>
                {
                    // TODO: support more than one device?
                    CargoClient = device;

                    IsConnected = true;

                    //TODO: call an "OnConnected" function

                    // Bluetooth is sensitive to multiple things going on at once across different threads
                    // so let's make sure we go about this serially

                    Properties = new BandProperties(CargoClient);
                    await Properties.InitAsync();

                    Theme = new BandTheme(CargoClient);
                    await Theme.InitAsync();

                    Sensors = new BandSensors(CargoClient);
                    await Sensors.InitAsync();

                    Tiles = new BandTiles(CargoClient);
                    await Tiles.InitAsync();
                }));
            }
        }

        private static async Task<IBandInfo[]> GetConnectedDevicesAsync()
        {
            var devices = new List<IBandInfo>();

            devices.AddRange(await GetConnectedUSBDevicesAsync());
            devices.AddRange(await GetConnectedBluetoothDevicesAsync());

            return devices.ToArray();
        }

        private static async Task<IBandInfo[]> GetConnectedUSBDevicesAsync()
        {
            return await BandAdminClientManager.Instance.GetBandsAsync();
        }

        private static async Task<IBandInfo[]> GetConnectedBluetoothDevicesAsync()
        {
            return new IBandInfo[] {};
                // Temporary BT removal: await CargoClientExtender.BluetoothClient.GetConnectedDevicesAsync();
        }

        private async Task<ICargoClient> GetUsbBand()
        {
            var devices = await GetConnectedUSBDevicesAsync();

            if (devices != null && devices.Length > 0)
            {
                _deviceInfo = devices[0];

                return (await BandAdminClientManager.Instance.ConnectAsync(_deviceInfo)) as ICargoClient;
            }

            return null;
        }

        private bool DesktopSyncAppIsRunning()
        {
            return IsDesktopSyncAppRunning = (Process.GetProcessesByName("Microsoft Band Sync").Length > 0);
        }

        private void InitializeCargoLogging()
        {
            // get log instance
            var field = typeof (Logger).GetField("traceListenerInternal", BindingFlags.Static | BindingFlags.NonPublic);
            field.SetValue(null, BandLogger.Instance);
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