using System;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using EasyScreenshot.Services;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;

namespace EasyScreenshot
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer;
        private ConnectionService _connectionService = new ConnectionService();
        private KeyPressService _keyPressService = new KeyPressService();
        private System.Windows.Forms.NotifyIcon _notifyIcon = new System.Windows.Forms.NotifyIcon();


        public MainWindow()
        {
            InitializeComponent();
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 10);
            timer.Tick += Ping;
           
            var iconPath = Environment.CurrentDirectory + "\\ScreenshotIcon.ico";
            _notifyIcon.Icon = new System.Drawing.Icon(iconPath);
            _notifyIcon.Visible = true;
            _notifyIcon.Click +=
                delegate (object sender, EventArgs args)
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
        }

        private async void Ping(object sender, EventArgs e)
        {
            string response = await _connectionService.GetIncomingConnections(YourId.Text);

            if (string.IsNullOrEmpty(response))
            {
                return;
            }

            var result = System.Windows.MessageBox.Show("Incoming screenshot. Do you want to look?",
                                            "Screenshot",
                                            MessageBoxButton.YesNo,
                                            MessageBoxImage.Information
                                            );

            if (result.ToString() == "Yes")
            {
                Uri uri = _connectionService.GetUrlFromResponse(response);
                ShowScreenshot(uri);
            }
        }

        private async void SendScreenshotButton_Click(object sender, RoutedEventArgs e)
        {
            bool receiverExists = await _connectionService.ReceiverExists(PartnerId.Text);
            if (!receiverExists)
            {
                System.Windows.MessageBox.Show("Partner was not found",
                                                "Error",
                                                MessageBoxButton.OK,
                                                MessageBoxImage.Error);
                return;
            }

            Thread.Sleep(200);
            WindowState = WindowState.Minimized;
            Thread.Sleep(200);

            string url = string.Empty;
            using (var memoryStreamScreenshot = new MemoryStream())
            {
                _connectionService.GetScreenShot(memoryStreamScreenshot);
                url = await _connectionService.UploadScreenshotToImageHosting(memoryStreamScreenshot);
                //ShowScreenshot(memoryStreamScreenshot);
            }

            await _connectionService.CreateConnection(url, YourId.Text, PartnerId.Text);
            _notifyIcon.ShowBalloonTip(2000, "Done", "Image was successfully sent", ToolTipIcon.Info);
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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            YourId.Text = await _connectionService.Register();
            timer.Start();

            _keyPressService.HookedKeys.Add(Keys.PrintScreen);
            _keyPressService.KeyDown += new System.Windows.Forms.KeyEventHandler(HiddenWindow_KeyDown);
        }

        private async void HiddenWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.PrintScreen)
            {
                string url = string.Empty;
                using (var memoryStreamScreenshot = new MemoryStream())
                {
                    _connectionService.GetScreenShot(memoryStreamScreenshot);
                    url = await _connectionService.UploadScreenshotToImageHosting(memoryStreamScreenshot);
                    _notifyIcon.ShowBalloonTip(2000, "Image url was copied", "Press CTRL+V to insert image link", ToolTipIcon.Info);
                }

                url = string.Format(ConnectionService.ImgUrlTemplate, url);
                System.Windows.Forms.Clipboard.SetText(url);
            }
        }

        private async void Window_Closed(object sender, EventArgs e)
        {
            await _connectionService.Unregister(YourId.Text);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendScreenshotButton_Click(sender, null);
            }
        }
    }
}
