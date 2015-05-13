using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using unBand.Cloud;

namespace unBand.pages
{
    /// <summary>
    ///     Interaction logic for AboutPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl, INotifyPropertyChanged
    {
        private string _heading;

        public SettingsPage(bool updated = false)
        {
            InitializeComponent();

            DataContext = About.Current;

            Heading = updated ? "unBand has been updated! Here's what you got:" : "Changelog";
        }

        public string Heading
        {
            get { return _heading; }
            set
            {
                if (_heading != value)
                {
                    _heading = value;

                    NotifyPropertyChanged();
                }
            }
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }

        private async void ButtonClearLoginInfo_Click(object sender, RoutedEventArgs e)
        {
            new BandCloudClient().Logout();

            await
                ((MetroWindow) (Window.GetWindow(this))).ShowMessageAsync("Restart Required",
                    "We'll now restart unBand to flush your login session...");

            Relaunch();
        }

        // TODO: This should probably live in a utils lib
        private void Relaunch()
        {
            Process rv = null;
            var processInfo = new ProcessStartInfo(Assembly.GetExecutingAssembly().CodeBase);

            // relaunch with runas to get elevated
            processInfo.UseShellExecute = true;

            try
            {
                rv = Process.Start(processInfo);

                Application.Current.Shutdown();
            }
            catch (Exception)
            {
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