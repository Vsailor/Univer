using COM.Chat.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace COM.Chat.Client.Presentation.Services.Abstract
{
    public interface IUserPresentationService
    {
        Task RegisterUser(string login, string password);
        Task<User> GetUserByLoginAsync(string login);
        User GetUserByLogin(string login);
        List<string> GetUsersLogins();
    }
}
