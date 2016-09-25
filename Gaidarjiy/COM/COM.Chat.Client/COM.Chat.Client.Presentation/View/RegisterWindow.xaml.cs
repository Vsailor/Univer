using COM.Chat.Client.Models;
using COM.Chat.Client.Presentation.Services;
using COM.Chat.Client.Presentation.Services.Abstract;
using System.Threading.Tasks;
using System.Windows;

namespace COM.Chat.Client.Presentation.View
{
    public partial class RegisterWindow : Window
    {
        private IUserPresentationService _userPresentationService;
        public RegisterWindow()
        {
            _userPresentationService = new UserPresentationService();
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

            await RegisterUserAsync(LoginField.Text, PasswordField.Text);
        }

        private async Task RegisterUserAsync(string login, string password)
        {
            var registered = await Task.Run(() =>
            {
                User user = _userPresentationService.GetUserByLogin(login);
                if (user != null)
                {
                    MessageBox.Show("This user is already exists", "Declined", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return false;
                }

                _userPresentationService.RegisterUser(login, password);

                MessageBox.Show("User was registered successful", "Done", MessageBoxButton.OK, MessageBoxImage.Information);
                return true;
            });

            if (registered)
            {
                Close();
            }
        }
    }
}