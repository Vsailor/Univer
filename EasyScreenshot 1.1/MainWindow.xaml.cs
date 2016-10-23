using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using EasyScreenshot.Models;
using EasyScreenshot.Services;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using MessageBox = System.Windows.Forms.MessageBox;

namespace EasyScreenshot
{
    public partial class MainWindow : Window
    {
        private readonly ConnectionService _connectionService = new ConnectionService();
        private readonly KeyPressService _keyPressService = new KeyPressService();
        private readonly NotifyIcon _notifyIcon = new System.Windows.Forms.NotifyIcon();
        private readonly BackgroundWorker _backgroundWorker = new BackgroundWorker();
        private Translation.Languages _language = Translation.Languages.English;
        private readonly Dictionary<Keys, Work> _keyWorkBinding = new Dictionary<Keys, Work>();
        private ImageFormat _imageFormat;

        private enum Work
        {
            CreateAndUploadHotkey,
            CreateAndSaveHotkey,
            CreateAndUseSnippingToolHotkey
        }

        public MainWindow()
        {
            InitializeComponent();
            _keyPressService.KeyDown += HiddenWindow_KeyDown;

            var iconPath = Environment.CurrentDirectory + "\\ScreenshotIcon.ico";
            _imageFormat = ImageFormat.Png;
            _notifyIcon.Icon = new System.Drawing.Icon(iconPath);
            _notifyIcon.Visible = true;
            _notifyIcon.Click +=
                delegate
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };

            _backgroundWorker.DoWork += UploadScreenshot;
            _backgroundWorker.RunWorkerCompleted += ScreenshotUploaded;
            //Hide();

            _notifyIcon.ShowBalloonTip(
                2000,
                Translation.GetTranslation(Translation.Phrases.ReadyToMakeScreenshot, _language),
                Translation.GetTranslation(Translation.Phrases.ClickToCameraIcon, _language), ToolTipIcon.Info);

            if (File.Exists(Environment.CurrentDirectory + "/" + ConfigurationManager.AppSettings["ConfigPath"]))
            {
                LoadSettings();
                FillLanguageCombobox();
                LoadTranslations();
                LoadComboboxTranslations();
            }
            else
            {
                PathForScreenshotsTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                FillLanguageCombobox();
                LanguageCombobox.SelectedIndex = 2;
            }
        }

        private void FillLanguageCombobox()
        {
            LanguageCombobox.Items.Clear();
            LanguageCombobox.Items.Add(Translation.GetTranslation(Translation.Phrases.Ukrainian, _language));
            LanguageCombobox.Items.Add(Translation.GetTranslation(Translation.Phrases.Russian, _language));
            LanguageCombobox.Items.Add(Translation.GetTranslation(Translation.Phrases.English, _language));
        }

        private void LoadSettings()
        {
            string[] settings = File.ReadAllLines(Environment.CurrentDirectory + "/" + ConfigurationManager.AppSettings["ConfigPath"]);

            try
            {
                CreateAndUploadHotkey.Text = GetValueFromSetting(Work.CreateAndUploadHotkey.ToString(), settings);
                CreateAndSaveHotkey.Text = GetValueFromSetting(Work.CreateAndSaveHotkey.ToString(), settings);
                //CreateAndUseSnippingToolHotkey.Text = GetValueFromSetting(Work.CreateAndUseSnippingToolHotkey.ToString(), settings);
                PathForScreenshotsTextBox.Text = GetValueFromSetting("PathForScreenshots", settings);
                if (!Directory.Exists(PathForScreenshotsTextBox.Text))
                {
                    PathForScreenshotsTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                }

                _language = (Translation.Languages)(Enum.Parse(typeof(Translation.Languages), GetValueFromSetting("Language", settings)));
                SelectFormat(GetValueFromSetting("Format", settings));

                BindHotkeys();
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(
                    ex.Message,
                    Translation.GetTranslation(Translation.Phrases.Error, _language),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                PathForScreenshotsTextBox.Text = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            }
        }

        private void SelectFormat(string format)
        {
            _imageFormat = GetFormatFromString(format);

            if (_imageFormat == ImageFormat.Png)
            {
                PngRB.IsChecked = true;
            }
            else if (_imageFormat == ImageFormat.Jpeg)
            {
                JpegRB.IsChecked = true;
            }
            else if (_imageFormat == ImageFormat.Bmp)
            {
                BmpRB.IsChecked = true;
            }
            else if (_imageFormat == ImageFormat.Gif)
            {
                GifRB.IsChecked = true;
            }
        }

        private string GetValueFromSetting(string key, string[] settings)
        {
            var setting = settings.FirstOrDefault(s => s.Contains(key));
            if (setting == null)
            {
                throw new ArgumentNullException("Can't find setting");
            }

            return setting.Remove(0, setting.IndexOf(":") + 2);
        }

        private void ScreenshotUploaded(object sender, RunWorkerCompletedEventArgs e)
        {
            string url = string.Format(ConnectionService.ImgUrlTemplate, e.Result as string);
            System.Windows.Forms.Clipboard.SetText(url);
            _notifyIcon.ShowBalloonTip(
                2000,
                Translation.GetTranslation(Translation.Phrases.ImageUrlWasCopied, _language),
                Translation.GetTranslation(Translation.Phrases.PressToInsertImageLink, _language),
                ToolTipIcon.Info);
        }

        private void HiddenWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (!_keyWorkBinding.ContainsKey(e.KeyCode))
                return;

            if (_keyWorkBinding[e.KeyCode] == Work.CreateAndUploadHotkey)
            {
                using (var memoryStreamScreenshot = new MemoryStream())
                {
                    _connectionService.GetScreenShot(memoryStreamScreenshot);
                    _backgroundWorker.RunWorkerAsync(memoryStreamScreenshot);
                }
            }
            else if (_keyWorkBinding[e.KeyCode] == Work.CreateAndSaveHotkey)
            {
                using (var memoryStreamScreenshot = new MemoryStream())
                {
                    _connectionService.GetScreenShot(memoryStreamScreenshot);

                    Image image = Image.FromStream(memoryStreamScreenshot);
                    string fileName =
                        string.Format(
                            "\\{0}-{1}.{2}",
                            Translation.GetTranslation(Translation.Phrases.Screenshot, _language),
                            DateTime.Now.ToString("dd-MM-yyyy-hh-mm-ss-fff"),
                            _imageFormat.ToString().ToLower());

                    image.Save(PathForScreenshotsTextBox.Text + fileName, _imageFormat);
                    _notifyIcon.ShowBalloonTip(
                        2000,
                        Translation.GetTranslation(Translation.Phrases.Done, _language),
                        string.Format("{0} \"{1}\"",
                            Translation.GetTranslation(Translation.Phrases.ImageWasSaved, _language),
                            PathForScreenshotsTextBox.Text.Remove(0, PathForScreenshotsTextBox.Text.LastIndexOf("\\") + 1)),
                            ToolTipIcon.Info);
                    memoryStreamScreenshot.Close();
                }
            }
        }

        private void UploadScreenshot(object sender, DoWorkEventArgs e)
        {
            var memoryStreamScreenshot = e.Argument as MemoryStream;
            if (memoryStreamScreenshot == null)
            {
                throw new ArgumentException("Argument is not a memory stream");
            }

            e.Result = _connectionService.UploadScreenshotToImageHosting(memoryStreamScreenshot);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
                Hide();
        }

        private new void PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var textbox = sender as System.Windows.Controls.TextBox;
            textbox.Text = GetKeyPressed(sender, e);
        }

        private string GetKeyPressed(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // The text box grabs all input.
            e.Handled = true;

            // Fetch the actual shortcut key.
            Key key = (e.Key == Key.System ? e.SystemKey : e.Key);

            // Ignore modifier keys.
            if (key == Key.LeftShift || key == Key.RightShift
                || key == Key.LeftCtrl || key == Key.RightCtrl
                || key == Key.LeftAlt || key == Key.RightAlt
                || key == Key.LWin || key == Key.RWin)
            {
                return string.Empty;
            }

            return key.ToString();
        }

        private void PathForScreenshotsTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog();
            var result = folderBrowserDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                PathForScreenshotsTextBox.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string settings = string.Empty;
                if (CreateAndUploadHotkey.Text == CreateAndSaveHotkey.Text)
                {
                    throw new ArgumentException();
                }

                Translation.Languages newLanguage = (Translation.Languages)LanguageCombobox.SelectedIndex;
                settings += Work.CreateAndUploadHotkey + ": " + CreateAndUploadHotkey.Text + Environment.NewLine;
                settings += Work.CreateAndSaveHotkey + ": " + CreateAndSaveHotkey.Text + Environment.NewLine;
                //settings += Work.CreateAndUseSnippingToolHotkey + ": " + CreateAndUseSnippingToolHotkey.Text + Environment.NewLine;
                settings += string.Format("PathForScreenshots: {0}" + Environment.NewLine,
                    PathForScreenshotsTextBox.Text);
                settings += string.Format("Language: {0}" + Environment.NewLine, newLanguage);
                settings += string.Format("Format: {0}" + Environment.NewLine, _temporaryFormat ?? _imageFormat);
                _language = newLanguage;
                _imageFormat = _temporaryFormat;

                File.WriteAllText(Environment.CurrentDirectory + "/" + ConfigurationManager.AppSettings["ConfigPath"],
                    settings);
                BindHotkeys();
                LoadTranslations();
                LoadComboboxTranslations();
                MessageBox.Show(
                    Translation.GetTranslation(Translation.Phrases.AllSettingsSaved, _language),
                    Translation.GetTranslation(Translation.Phrases.Done, _language),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (ArgumentException)
            {
                MessageBox.Show(
                    Translation.GetTranslation(Translation.Phrases.HotkeyNotSupported, _language),
                    Translation.GetTranslation(Translation.Phrases.Error, _language),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BindHotkeys()
        {
            _keyPressService.HookedKeys.Clear();
            _keyWorkBinding.Clear();

            Keys key;
            if (!string.IsNullOrEmpty(CreateAndUploadHotkey.Text))
            {
                key = (Keys)KeyInterop.VirtualKeyFromKey(
                    (Key)Enum.Parse(typeof(Key), CreateAndUploadHotkey.Text));
                _keyPressService.HookedKeys.Add(key);
                _keyWorkBinding.Add(key, Work.CreateAndUploadHotkey);
            }

            if (!string.IsNullOrEmpty(CreateAndSaveHotkey.Text))
            {
                key = (Keys)KeyInterop.VirtualKeyFromKey(
                    (Key)Enum.Parse(typeof(Key), CreateAndSaveHotkey.Text));
                _keyPressService.HookedKeys.Add(key);
                _keyWorkBinding.Add(key, Work.CreateAndSaveHotkey);
            }

            //if (!string.IsNullOrEmpty(CreateAndUseSnippingToolHotkey.Text))
            //{
            //    key = (Keys)KeyInterop.VirtualKeyFromKey(
            //        (Key) Enum.Parse(typeof (Key), CreateAndUseSnippingToolHotkey.Text));
            //    _keyPressService.HookedKeys.Add(key);
            //    _keyWorkBinding.Add(key, Work.CreateAndUseSnippingToolHotkey);
            //}
        }

        private void LoadTranslations()
        {
            Hotkeys.Text = Translation.GetTranslation(Translation.Phrases.Hotkeys, _language);
            CreateAndUpload.Text = Translation.GetTranslation(Translation.Phrases.CreateAndUpload, _language);
            CreateAndSave.Text = Translation.GetTranslation(Translation.Phrases.CreateAndSave, _language);
            //CreateAndUseSnippingTool.Text = Translation.GetTranslation(Translation.Phrases.CreateAndUseSnippingTool, _language);
            PathForScreenshots.Text = Translation.GetTranslation(Translation.Phrases.PathForScreenshots, _language);
            LanguageTextBox.Text = Translation.GetTranslation(Translation.Phrases.Language, _language);
            SaveButton.Content = Translation.GetTranslation(Translation.Phrases.Save, _language);
            FormatTextBox.Text = Translation.GetTranslation(Translation.Phrases.Format, _language);
        }

        private void LoadComboboxTranslations()
        {
            LanguageCombobox.Items[0] = Translation.GetTranslation(Translation.Phrases.English, _language);
            LanguageCombobox.Items[1] = Translation.GetTranslation(Translation.Phrases.Ukrainian, _language);
            LanguageCombobox.Items[2] = Translation.GetTranslation(Translation.Phrases.Russian, _language);

            LanguageCombobox.SelectedIndex = Translation.GetIndex(_language);
        }

        private void CreateAndUploadClear_Click(object sender, RoutedEventArgs e)
        {
            CreateAndUploadHotkey.Text = string.Empty;
        }

        private void CreateAndSaveClear_Click(object sender, RoutedEventArgs e)
        {
            CreateAndSaveHotkey.Text = string.Empty;
        }

        private void CreateAndUseSnippingToolClear_Click(object sender, RoutedEventArgs e)
        {
            //CreateAndUseSnippingToolHotkey.Text = string.Empty;
        }

        private ImageFormat _temporaryFormat;
        private void ImageFormatRadioButtonChecked(object sender, RoutedEventArgs e)
        {
            var button = sender as System.Windows.Controls.RadioButton;
            _temporaryFormat = GetFormatFromString(button.Content.ToString());
        }

        private ImageFormat GetFormatFromString(string format)
        {
            if (format.ToLower().Contains("png"))
                return ImageFormat.Png;
            if (format.ToLower().Contains("jpg") || format.ToLower().Contains("jpeg"))
                return ImageFormat.Jpeg;
            if (format.ToLower().Contains("bmp"))
                return ImageFormat.Bmp;
            if (format.ToLower().Contains("gif"))
                return ImageFormat.Gif;

            return null;
        }
    }
}
