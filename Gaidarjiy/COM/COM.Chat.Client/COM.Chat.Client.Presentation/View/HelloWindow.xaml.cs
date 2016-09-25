using System.Windows;

namespace COM.Chat.Client.Presentation.View
{
    public partial class HelloWindow : Window
    {
        public HelloWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            var helloWindow = new RegisterWindow();
            helloWindow.Show();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            var loginWindow = new LoginWindow(this);
            loginWindow.Show();
        }
    }
}
