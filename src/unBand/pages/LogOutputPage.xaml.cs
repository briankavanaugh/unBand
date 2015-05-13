using System.Windows.Controls;
using unBand.BandHelpers;

namespace unBand.pages
{
    /// <summary>
    ///     Interaction logic for MyBandPage.xaml
    /// </summary>
    public partial class LogOutputPage : UserControl
    {
        private readonly BandManager _band;

        public LogOutputPage()
        {
            InitializeComponent();

            _band = BandManager.Instance;

            DataContext = _band;
        }
    }
}