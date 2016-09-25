using COM.Chat.Client.Models;
using COM.Chat.Client.Presentation.Services;
using COM.Chat.Client.Presentation.Services.Abstract;
using System.Threading.Tasks;
using System.Windows;

namespace COM.Chat.Client.Presentation.View
{
    public partial class LoginWindow : Window
    {
        private IUserPresentationService _userPresentationService;
        private Window _mainWindow;

        public LoginWindow(Window mainWindow)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _userPresentationService = new UserPresentationService();
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            User user = await _userPresentationService.GetUserByLoginAsync(LoginField.Text);
            if (user == null)
            {
                MessageBox.Show("This user has not been found", "Declined", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (user.Password != PasswordField.Text)
            {
                MessageBox.Show("Login or password is incorrect", "Declined", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _mainWindow.Close();
            var chatWindow = new ChatWindow(user, _userPresentationService);
            chatWindow.Show();
            Close();
        }
    }
}
