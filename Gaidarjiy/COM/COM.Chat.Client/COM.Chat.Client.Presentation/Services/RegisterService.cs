using COM.Chat.Client.Presentation.Services.Abstract;
using COM.Chat.Client.Services;
using COM.Chat.Client.Services.Abstract;
using System.Threading.Tasks;
using COM.Chat.Client.Models;

namespace COM.Chat.Client.Presentation.Services
{
    public class RegisterService : IRegisterService
    {
        IUserService _userService;

        public RegisterService()
        {
            _userService = new UserService();
        }

        public User GetUserByLogin(string login)
        {
            return _userService.GetUserByLogin(login);
        }

        public async Task RegisterUser(string login, string password)
        {
            await _userService.RegisterUser(login, password);
        }
    }
}
