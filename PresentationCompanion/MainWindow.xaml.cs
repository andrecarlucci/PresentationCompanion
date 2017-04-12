using SharpSenses;
using SharpSenses.RealSense.Capabilities;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace PresentationCompanion {
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            MouseDown += MainWindow_MouseDown;
            Loaded += MainWindow_Loaded;

            var camera = Camera.Create(Capability.SegmentationStreamTracking);
            camera.SegmentationStream.NewImageAvailable += SegmentationStream_NewImageAvailable;
            camera.Start();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e) {
            var desktopWorkingArea = SystemParameters.WorkArea;
            Left = desktopWorkingArea.Right - Width;
            Top = desktopWorkingArea.Bottom - Height;
        }

        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left) {
                DragMove();
            }
        }

        private void SegmentationStream_NewImageAvailable(object sender, ImageEventArgs e) {
            Bitmap bitmap;
            using (var mem = new MemoryStream(e.BitmapImage)) {
                bitmap = new Bitmap(mem);
            }
            bitmap.MakeTransparent(Color.Black);
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => {
                Video.Source = ToWpfBitmap(bitmap);
            }));
        }

        public BitmapSource ToWpfBitmap(Bitmap bitmap) {
            using (MemoryStream stream = new MemoryStream()) {
                bitmap.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                BitmapImage result = new BitmapImage();
                result.BeginInit();
                result.CacheOption = BitmapCacheOption.OnLoad;
                result.StreamSource = stream;
                result.EndInit();
                result.Freeze();
                return result;
            }
        }
    }
}
