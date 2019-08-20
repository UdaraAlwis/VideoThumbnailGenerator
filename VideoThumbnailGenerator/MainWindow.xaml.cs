using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VideoThumbnailGenerator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> filePaths = new List<string> (){ };

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            filePaths = Directory.GetFiles(@"C:\Test\", "*.mp4").ToList();
        }

        void ImportMedia(string mediaFilePathName, int waitTime, int position)
        {
            MediaPlayer player = new MediaPlayer { Volume = 0, ScrubbingEnabled = true };
            player.Open(new Uri(mediaFilePathName));
            player.Pause();
            player.Position = TimeSpan.FromSeconds(position);
            //We need to give MediaPlayer some time to load. 
            //The efficiency of the MediaPlayer depends                 
            //upon the capabilities of the machine it is running on and 
            //would be different from time to time
            System.Threading.Thread.Sleep(waitTime * 1000);

            //120 = thumbnail width, 90 = thumbnail height and 96x96 = horizontal x vertical DPI
            //In an real application, you would not probably use hard coded values!
            RenderTargetBitmap rtb = new RenderTargetBitmap(player.NaturalVideoWidth, player.NaturalVideoHeight, 0, 0, PixelFormats.Pbgra32);
            DrawingVisual dv = new DrawingVisual();
            using (DrawingContext dc = dv.RenderOpen())
            {
                dc.DrawVideo(player, new Rect(0, 0, player.NaturalVideoWidth, player.NaturalVideoHeight));
            }
            rtb.Render(dv);

            BitmapFrame frame = BitmapFrame.Create(rtb).GetCurrentValueAsFrozen() as BitmapFrame;
            BitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(frame as BitmapFrame);
            MemoryStream memoryStream = new MemoryStream();
            encoder.Save(memoryStream);

            // set the position of stream to 0 after writing to it
            memoryStream.Seek(0, System.IO.SeekOrigin.Begin);

            var img = new BitmapImage();
            img.BeginInit();
            img.StreamSource = memoryStream;
            img.EndInit();
            ImageScreenshot.Source = img;

            var pathName = new Uri(mediaFilePathName);

            Save(img, $"{DateTime.Now.Ticks}.jpg");

            // ImageScreenshot.Source = BitmapFrame.Create(memoryStream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

            //Here we have the thumbnail in the MemoryStream!
            player.Close();
        }

        public void Save(BitmapImage image, string filePath)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(image));

            using (var fileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create))
            {
                encoder.Save(fileStream);
            }
        }

        /// <summary>
        /// Snippet extracted from
        /// https://www.codeproject.com/Tips/496864/Getting-Thumbnail-from-Video-using-MediaPlayer-Cla
        /// </summary>
        private void CaptureThumbnail_MediaPlayer()
        {
            // choose a random file from the list of files
            Random random = new Random();
            var chosenFile = filePaths[random.Next(filePaths.Count)];

            // capture the thumbnail
            ImageScreenshot.Source = null;
            Debug.WriteLine(chosenFile);
            ImportMedia(chosenFile, 10, 18);
        }

        /// <summary>
        /// Snippet extracted from
        /// https://gist.github.com/kad1r/6721178
        /// https://ffmpeg.org/download.html
        /// </summary>
        private void CaptureThumbnail_FFMPegLibrary()
        {
            // choose a random file from the list of files
            Random random = new Random();
            var chosenFile = filePaths[random.Next(filePaths.Count)];

            // capture the thumbnail
            ImageScreenshot.Source = null;
            Debug.WriteLine(chosenFile);
            GenerateThumbnailMethod2Class.GenerateThumbnailFromVideo(chosenFile);
        }

        /// <summary>
        /// Snippet extracted from
        /// https://xabe.net/net-video-converter-toturial/
        /// </summary>
        private async void CaptureThumbnail_XabeFFMPegLibrary()
        {
            // choose a random file from the list of files
            Random random = new Random();
            var chosenFile = filePaths[random.Next(filePaths.Count)];

            // capture the thumbnail
            ImageScreenshot.Source = null;
            Debug.WriteLine(chosenFile);
            await GenerateThumbnailMethod3Class.GenerateThumbnailFromVideo(chosenFile);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // CaptureThumbnail_MediaPlayer();
            // CaptureThumbnail_FFMPegLibrary();
            CaptureThumbnail_XabeFFMPegLibrary();
        }
    }
}
