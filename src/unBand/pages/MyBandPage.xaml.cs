using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using unBand.BandHelpers;

namespace unBand.pages
{
    /// <summary>
    ///     Interaction logic for MyBandPage.xaml
    /// </summary>
    public partial class MyBandPage : UserControl
    {
        private readonly BandManager _band;

        public MyBandPage()
        {
            InitializeComponent();

            _band = BandManager.Instance;

            DataContext = _band;
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}