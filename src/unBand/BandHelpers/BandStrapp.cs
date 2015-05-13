using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Band;
using Microsoft.Band.Admin;
using Microsoft.Band.Tiles;

namespace unBand.BandHelpers
{
    internal class BandStrapp : INotifyPropertyChanged
    {
        // Starbucks have a magic template all to themselves (at least, I haven't found a way to apply it
        // to others / edit it), so let's detect that
        private static readonly Guid STARBUCKS_GUID = new Guid("{64a29f65-70bb-4f32-99a2-0f250a05d427}");
        private readonly BandTiles _tiles;

        public BandStrapp(BandTiles tiles, AdminBandTile strapp)
        {
            Strapp = strapp;
            _tiles = tiles;

            TileImage = strapp.Image.ToWriteableBitmap();
        }

        public AdminBandTile Strapp { get; private set; }
        public WriteableBitmap TileImage { get; private set; }

        public bool IsDefault
        {
            get { return _tiles.DefaultStrapps.Any(i => i.TileId == Strapp.TileId); }
        }

        public bool IsStarbucks
        {
            get { return Strapp.TileId == STARBUCKS_GUID; }
        }

        internal void SetIcon(string fileName)
        {
            var bitmapSource = new BitmapImage();

            bitmapSource.BeginInit();
            bitmapSource.UriSource = new Uri(fileName);

            // ensure that we resize to the correct dimensions
            bitmapSource.DecodePixelHeight = 46;
            bitmapSource.DecodePixelWidth = 46;

            bitmapSource.EndInit();

            var pbgra32Image = new FormatConvertedBitmap(bitmapSource, PixelFormats.Pbgra32, null, 0);

            var bmp = new WriteableBitmap(pbgra32Image);

            var images = new List<BandIcon> {bmp.ToBandIcon(), bmp.ToBandIcon()};

            Strapp.SetImageList(Strapp.TileId, images, 0);

            _tiles.UpdateStrapp(Strapp);

            // this should refresh all bindings into the Strapp (in particular, the image)
            NotifyPropertyChanged("Strapp");
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