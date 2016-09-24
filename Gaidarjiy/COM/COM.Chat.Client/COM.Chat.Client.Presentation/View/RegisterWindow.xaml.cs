using COM.Chat.Client.Presentation.Services;
using COM.Chat.Client.Presentation.Services.Abstract;
using System.Windows;

namespace COM.Chat.Client.Presentation.View
{
    public partial class RegisterWindow : Window
    {
        private IRegisterService _registerService;
        public RegisterWindow()
        {
            _registerService = new RegisterService();
            InitializeComponent();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LoginField.Text))
            {
                MessageBox.Show("Login is empty");
                return;
            }

            if (string.IsNullOrEmpty(PasswordField.Text))
            {
                MessageBox.Show("Password is empty");
                return;
            }

            await _registerService.RegisterUser(LoginField.Text, PasswordField.Text);
        }
    }
}