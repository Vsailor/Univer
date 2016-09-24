using COM.Chat.Client.Data;
using COM.Chat.Client.Data.Abstract;
using COM.Chat.Client.Services.Abstract;
using System.Threading.Tasks;

namespace COM.Chat.Client.Services
{
    public class UserService : IUserService
    {
        private IUserRepository _userRepository;

        public UserService()
        {
            _userRepository = new UserRepository();
        }

        public async Task RegisterUser(string login, string password)
        {
            await _userRepository.RegisterUser(login, password, 0);
        }
    }
}
