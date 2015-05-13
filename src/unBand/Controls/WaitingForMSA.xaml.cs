using System.Windows;
using System.Windows.Controls;
using unBand.CloudHelpers;

namespace unBand.Controls
{
    /// <summary>
    ///     Interaction logic for WaitingForBand.xaml
    /// </summary>
    public partial class WaitingForMSA : UserControl
    {
        public WaitingForMSA()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            BandCloudManager.Instance.Login();
        }
    }
}