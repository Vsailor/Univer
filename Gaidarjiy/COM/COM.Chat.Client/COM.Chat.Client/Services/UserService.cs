using COM.Chat.Client.Data;
using COM.Chat.Client.Data.Abstract;
using COM.Chat.Client.Models;
using COM.Chat.Client.Services.Abstract;
using System.Linq;
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

        public Task<User> GetUserByLogin(string login)
        {
            var users = _userRepository.GetUserByLogin(login, 0);
            return Task.Run(() => users.FirstOrDefault());
        }
    }
}
