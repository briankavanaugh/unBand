using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using unBand.BandHelpers;

namespace unBand.pages
{
    /// <summary>
    ///     Interaction logic for ThemePage.xaml
    /// </summary>
    public partial class ThemePage : UserControl
    {
        private readonly BandManager _band;

        public ThemePage()
        {
            InitializeComponent();

            _band = BandManager.Instance;

            DataContext = _band;
        }

        private void Background_MouseUp(object sender, MouseButtonEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Images|*.jpg;*.jpeg;*.png";

            if (dialog.ShowDialog() == true)
            {
                Telemetry.TrackEvent(TelemetryCategory.Theme, Telemetry.TelemetryEvent.ChangeBackground);
                _band.Theme.SetBackground(dialog.FileName);
            }
        }
    }
}