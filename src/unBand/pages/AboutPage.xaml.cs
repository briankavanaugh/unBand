using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace unBand.pages
{
    /// <summary>
    ///     Interaction logic for AboutPage.xaml
    /// </summary>
    public partial class AboutPage : UserControl, INotifyPropertyChanged
    {
        private string _heading;

        public AboutPage(bool updated = false)
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