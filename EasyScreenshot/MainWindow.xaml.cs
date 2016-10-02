using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Threading;
using System.Windows.Threading;
using EasyScreenshot.Models;

namespace EasyScreenshot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string AuthHeader = "Authorization: Client-ID 02aa55f72160611";
        private string imgUrlTemplate = "http://i.imgur.com/{0}.png";
        private DispatcherTimer timer;
        private string server = /*"http://localhost:51006/"*/"http://easyscreenshot.azurewebsites.net/";

        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 10);
            timer.Tick += OnTimerTick;
        }

        private async void OnTimerTick(object sender, EventArgs e)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(server + "api/connections/" + YourId.Text);
            request.Method = "GET";
            string responseString = string.Empty;
            await Task.Run(() =>
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
            });

            if (responseString.Contains("url"))
            {
                var result = System.Windows.MessageBox.Show("Incoming screenshot. Do you want to look?",
                                                "Screenshot",
                                                MessageBoxButton.YesNo,
                                                MessageBoxImage.Information
                                                );
                if (result.ToString() == "Yes")
                {
                    string url = responseString.Remove(0, 5);
                    url = url.Remove(url.IndexOf('&'));
                    ShowScreenshot(new Uri(string.Format(imgUrlTemplate, url)));
                }

                return;
            }
        }

        private async void SendScreenshotButton_Click(object sender, RoutedEventArgs e)
        {
            bool receiverExists = await ReceiverExists();
            if (!receiverExists)
            {
                System.Windows.MessageBox.Show("User was not found");
                return;
            }

            Thread.Sleep(200);
            WindowState = WindowState.Minimized;
            Thread.Sleep(200);
            string url = imgUrlTemplate;

            using (var memoryStreamScreenshot = new MemoryStream())
            {
                GetScreenShot(memoryStreamScreenshot);
                url = await UploadScreenshot(memoryStreamScreenshot);
                ShowScreenshot(memoryStreamScreenshot);
            }

            await CreateConnection(url);
        }

        private async Task<string> UploadScreenshot(MemoryStream screenShot)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("https://api.imgur.com/3/image");
            request.Headers.Add(AuthHeader);
            request.Method = "POST";
            using (var stream = request.GetRequestStream())
            {
                var arr = screenShot.ToArray();
                stream.Write(arr, 0, arr.Length);
            }

            return await Task.Run(() =>
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                response.Close();
                string start = "{\"data\":{\"id\":\"";
                responseString = responseString.Remove(responseString.IndexOf(start), start.Length);
                return responseString.Remove(responseString.IndexOf("\""));
            });
        }

        private void ShowScreenshot(MemoryStream stream)
        {
            var bitmapImg = new BitmapImage();

            var screenshotWindow = new ScreenshotWindow();

            bitmapImg.BeginInit();
            bitmapImg.StreamSource = stream;
            bitmapImg.EndInit();

            screenshotWindow.ScreenshotImage.Source = bitmapImg;

            screenshotWindow.Show();
            screenshotWindow.WindowState = WindowState.Maximized;
        }

        private void ShowScreenshot(Uri uri)
        {
            var bitmapImg = new BitmapImage(uri);
            var screenshotWindow = new ScreenshotWindow();
            screenshotWindow.ScreenshotImage.Source = bitmapImg;

            screenshotWindow.Show();
            screenshotWindow.WindowState = WindowState.Maximized;
        }

        private void GetScreenShot(MemoryStream memoryStream)
        {
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            using (Graphics gr = Graphics.FromImage(bmp))
            {
                gr.CopyFromScreen(Screen.PrimaryScreen.Bounds.X, Screen.PrimaryScreen.Bounds.Y,
                    0, 0, Screen.PrimaryScreen.Bounds.Size, CopyPixelOperation.SourceCopy);
            }

            bmp.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
        }

        private async Task SendImage(int senderId, int receiverId, string imgUrl)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(server + "api/connections/unregister/" + YourId.Text);
            request.Method = "GET";
            await Task.Run(() =>
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            });
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            YourId.Text = await Register();
            timer.Start();
        }

        private async void Window_Closed(object sender, EventArgs e)
        {
            await Unregister();
        }

        private async Task<string> Register()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(server + "api/connections/register");
            request.Method = "GET";
            return await Task.Run(() => 
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                response.Close();
                return responseString;
            });
        }

        private async Task Unregister()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(server + "api/connections/unregister/" + YourId.Text);
            request.Method = "GET";
            await Task.Run(() =>
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            });
        }

        private async Task<bool> ReceiverExists()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(server + "api/connections/users/" + PartnerId.Text);
            request.Method = "GET";
            return await Task.Run(() =>
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                return responseString != "404";
            });
        }

        private async Task CreateConnection(string url)
        {
            string connectUrl = server + string.Format("api/connections/create/{0}/{1}/{2}", YourId.Text, PartnerId.Text, url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(connectUrl);
            request.Method = "GET";

            await Task.Run(() =>
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            });
        }
    }
}
