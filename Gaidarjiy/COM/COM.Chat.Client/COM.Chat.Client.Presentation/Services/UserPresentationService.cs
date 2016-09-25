using COM.Chat.Client.Presentation.Services.Abstract;
using System.Threading.Tasks;
using COM.Chat.Client.Models;
using COM.Chat.Client.Services;
using COM.Chat.Client.Services.Abstract;
using System;
using System.Collections.Generic;

namespace COM.Chat.Client.Presentation.Services
{
    public class UserPresentationService : IUserPresentationService
    {
        IUserService _userService;

        public UserPresentationService()
        {
            _userService = new UserService();
        }

        public async Task<User> GetUserByLoginAsync(string login)
        {
            return await Task.Run(() =>
            {
                return GetUserByLogin(login);
            });
        }

        public List<string> GetUsersLogins()
        {
            return _userService.GetUsersLogins();
        }

        public async Task RegisterUser(string login, string password)
        {
            await _userService.RegisterUser(login, password);
        }

        public User GetUserByLogin(string login)
        {
            return _userService.GetUserByLogin(login);
        }
    }
}
