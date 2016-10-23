using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Drawing.Imaging;

namespace EasyScreenshot.Services
{
    public class ConnectionService
    {
        public static readonly string ImgUrlTemplate = ConfigurationManager.AppSettings["ImgUrlTemplate"];

        private static readonly string UploadUrl = ConfigurationManager.AppSettings["UploadUrl"];

        private static readonly string AuthHeader = ConfigurationManager.AppSettings["AuthHeader"];

        public Uri GetUrlFromResponse(string response)
        {
            string url = response.Remove(0, 5);
            url = url.Remove(url.IndexOf('&'));
            return new Uri(string.Format(ImgUrlTemplate, url));
        }

        public string UploadScreenshotToImageHosting(MemoryStream screenShot)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(UploadUrl);
            request.Headers.Add(AuthHeader);
            request.Method = "POST";
            using (var stream = request.GetRequestStream())
            {
                var arr = screenShot.ToArray();
                stream.Write(arr, 0, arr.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            response.Close();
            string start = "{\"data\":{\"id\":\"";
            responseString = responseString.Remove(responseString.IndexOf(start), start.Length);
            return responseString.Remove(responseString.IndexOf("\""));
        }

        public void GetScreenShot(MemoryStream memoryStream)
        {
            var bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                gr.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y,
                    0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
            }

            var encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(Encoder.Quality, 1000L);

            bmp.Save(memoryStream, GetEncoderInfo("image/png"), encoderParameters);
        }

        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
    }
}
