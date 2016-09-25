using COM.Chat.Client.Models;
using COM.Chat.Client.Presentation.Services;
using COM.Chat.Client.Presentation.Services.Abstract;
using System;
using System.Windows;
using System.Linq;

namespace COM.Chat.Client.Presentation.View
{
    public partial class ChatWindow : Window
    {
        private User _currentUser;
        private string _currentReciever;
        private IUserPresentationService _userPresentationService;
        private IMessagePresentationService _messagePresentationService;

        public ChatWindow(User user, IUserPresentationService userPresentationService)
        {
            InitializeComponent();
            _currentUser = user;
            _userPresentationService = userPresentationService;
            _messagePresentationService = new MessagePresentationService();
            HelloHeader.Text = $@"Hi, {user.Login}";
            var users = _userPresentationService.GetUsersLogins().Where(login => login != user.Login);
            Recievers.Items.Clear();
            users.ToList().ForEach(item => Recievers.Items.Add(item));
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NewMessage.Text))
            {
                return;
            }

            DateTime currentTime = DateTime.Now;
            string newMessage = $@"[{currentTime.ToLongTimeString()}] You: {NewMessage.Text}";
            if (string.IsNullOrEmpty(Messages.Text))
            {
                Messages.Text = newMessage;
            }
            else
            {
                Messages.Text = newMessage + Environment.NewLine + Messages.Text;
            }

            if (_currentReciever != null)
            {
                var message = new Message()
                {
                    Content = NewMessage.Text,
                    ReceiverLogin = _currentReciever,
                    SenderLogin = _currentUser.Login,
                    Time = currentTime
                };

                _messagePresentationService.SendMessage(message);
                NewMessage.Text = string.Empty;
            }
        }

        private void Recievers_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            _currentReciever = Recievers.SelectedItem.ToString();
        }
    }
}
