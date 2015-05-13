using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using unBand.BandHelpers;

namespace unBand.pages
{
    /// <summary>
    ///     Interaction logic for MyBandPage.xaml
    /// </summary>
    public partial class TilesPage : UserControl
    {
        private readonly BandManager _band;

        public TilesPage()
        {
            InitializeComponent();

            _band = BandManager.Instance;

            DataContext = _band;
        }

        // Drag & Drop care of http://stackoverflow.com/a/4004590
        private void ListBox_Drop(object sender, DragEventArgs e)
        {
            var parent = sender as ListBox;
            var data = e.Data.GetData(typeof (BandStrapp)) as BandStrapp;
            var objectToPlaceBefore = GetObjectDataFromPoint(parent, e.GetPosition(parent)) as BandStrapp;

            if (data != null && objectToPlaceBefore != null)
            {
                var index = _band.Tiles.Strip.IndexOf(objectToPlaceBefore);

                _band.Tiles.Strip.Remove(data);
                _band.Tiles.Strip.Insert(index, data);
                parent.SelectedItem = data;
            }
        }

        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var parent = sender as ListBox;

            var data = GetObjectDataFromPoint(parent, e.GetPosition(parent)) as BandStrapp;

            if (data != null)
            {
                DragDrop.DoDragDrop(parent, data, DragDropEffects.Move);
            }
        }

        private static object GetObjectDataFromPoint(ListBox source, Point point)
        {
            var element = source.InputHitTest(point) as UIElement;
            if (element != null)
            {
                var data = DependencyProperty.UnsetValue;

                while (data == DependencyProperty.UnsetValue)
                {
                    data = source.ItemContainerGenerator.ItemFromContainer(element);

                    if (data == DependencyProperty.UnsetValue)
                        element = VisualTreeHelper.GetParent(element) as UIElement;

                    if (element == source)
                        return null;
                }

                if (data != DependencyProperty.UnsetValue)
                    return data;
            }

            return null;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            _band.Tiles.Save();
        }

        private void btnClearCounts_Click(object sender, RoutedEventArgs e)
        {
            _band.Tiles.ClearAllCounts();
        }

        private async void linkCustomInformation_Click(object sender, RoutedEventArgs e)
        {
            await ((MetroWindow) (Window.GetWindow(this))).ShowMessageAsync("Custom Tile Icons",
                @"For best results, use a single color, transparent 46x46 image. 

Any non-transparent pixels will be converted to white on the Band, so an image without transparency will just show up as a white box.
");
        }

        private void btnCustomizeIcon_Click(object sender, RoutedEventArgs e)
        {
            var bandStrapp = ((Button) sender).DataContext as BandStrapp;

            var dialog = new OpenFileDialog();
            dialog.Filter = "Images|*.jpg;*.jpeg;*.png";

            if (dialog.ShowDialog() == true)
            {
                Telemetry.TrackEvent(TelemetryCategory.Theme, Telemetry.TelemetryEvent.ChangeStrappIcon,
                    bandStrapp.Strapp.Name);

                bandStrapp.SetIcon(dialog.FileName);
            }
        }
    }
}