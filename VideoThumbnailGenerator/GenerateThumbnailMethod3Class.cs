using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using VideoThumbnailGenerator.Properties;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Model;

namespace VideoThumbnailGenerator
{
    public static class GenerateThumbnailMethod3Class
    {
        public static async Task<BitmapSource> GenerateThumbnailFromVideo(string videoFilePath)
        {
            BitmapSource thumbnailBitmapSource = null;

            try
            {
                string thumbnailFilePath = videoFilePath.Replace(".mp4", "_thumb.jpg");
                if (File.Exists(thumbnailFilePath))
                    File.Delete(thumbnailFilePath);

                //Set directory where app should look for FFmpeg 
                FFmpeg.ExecutablesPath =
                    @"C:\Users\udara\Downloads\ffmpeg-20190818-1965161-win64-static\ffmpeg-20190818-1965161-win64-static\bin\";

                FileInfo file = new FileInfo(videoFilePath);
                var mediaInfo = await MediaInfo.Get(file).ConfigureAwait(false);

                // grab a random timespan for a frame
                Random rand = new Random();
                var randomFrameValue = rand.Next(Convert.ToInt32(mediaInfo.Duration.TotalMilliseconds));

                IConversionResult result = await Conversion.Snapshot(videoFilePath, thumbnailFilePath, 
                                                                        TimeSpan.FromMilliseconds(randomFrameValue)).Start(); ;

                thumbnailBitmapSource = new BitmapImage(new Uri(thumbnailFilePath));
            }
            catch (Exception ex)
            {
                // ignored
                // or handle it :)
            }

            return thumbnailBitmapSource;
        }
    }
}
