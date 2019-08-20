using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace VideoThumbnailGenerator
{
    public static class GenerateThumbnailMethod2Class
    {
        public static BitmapSource GenerateThumbnailFromVideo(string videoFilePath)
        {
            BitmapSource thumbnailBitmapSource = null;

            try
            {
                string thumbnailFilePath = videoFilePath.Replace(".mp4", "_thumb.jpg");
                if (File.Exists(thumbnailFilePath))
                    File.Delete(thumbnailFilePath);

                var processInfo = new ProcessStartInfo();
                processInfo.FileName =
                    @"C:\Users\udara\Downloads\ffmpeg-20190818-1965161-win64-static\ffmpeg-20190818-1965161-win64-static\bin\ffmpeg.exe";
                processInfo.Arguments =
                    string.Format("-ss {0} -i {1} -f image2 -vframes 1 -y {2}", 5, "\"" + videoFilePath + "\"", "\"" + thumbnailFilePath + "\"");
                processInfo.CreateNoWindow = true;
                processInfo.UseShellExecute = false;
                using (var process = new Process())
                {
                    process.StartInfo = processInfo;
                    process.Start();
                    process.WaitForExit();
                }

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
