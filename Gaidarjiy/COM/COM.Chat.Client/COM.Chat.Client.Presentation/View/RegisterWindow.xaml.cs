using COM.Chat.Client.Models;
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
                MessageBox.Show("Login is empty", "Declined", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(PasswordField.Text))
            {
                MessageBox.Show("Password is empty", "Declined", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            User user = await _registerService.GetUserByLogin(LoginField.Text);
            if (user != null)
            {
                MessageBox.Show("This user is already exists", "Declined", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            await _registerService.RegisterUser(LoginField.Text, PasswordField.Text);
            MessageBox.Show("User was registered successful", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }
    }
}