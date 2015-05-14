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

            InitializeCargoLogging();
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
            _timer = new DispatcherTimer {Interval = TimeSpan.FromMilliseconds(1)};

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

                    if (devices.All(i => i.Name != Instance._deviceInfo.Name))
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
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    // TODO: support more than one device?
                    CargoClient = device;

                    IsConnected = true;
                }));
            }
        }

        private static async Task<IBandInfo[]> GetConnectedDevicesAsync()
        {
            var devices = new List<IBandInfo>();

            devices.AddRange(await GetConnectedUSBDevicesAsync());

            return devices.ToArray();
        }

        private static async Task<IBandInfo[]> GetConnectedUSBDevicesAsync()
        {
            return await BandAdminClientManager.Instance.GetBandsAsync();
        }

        private async Task<ICargoClient> GetUsbBand()
        {
            var devices = await GetConnectedUSBDevicesAsync();

            if (devices != null && devices.Length > 0)
            {
                _deviceInfo = devices[0];

                return (await BandAdminClientManager.Instance.ConnectAsync(_deviceInfo));
            }

            return null;
        }

        private bool DesktopSyncAppIsRunning()
        {
            return IsDesktopSyncAppRunning = (Process.GetProcessesByName("Microsoft Band Sync").Length > 0);
        }

        private static void InitializeCargoLogging()
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