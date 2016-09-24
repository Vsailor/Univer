using COM.Chat.Client.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace COM.Chat.Client.Data.Abstract
{
    interface IUserRepository
    {
        Task RegisterUser(string login, string password, byte isDeleted);
        List<User> GetUserByLogin(string login, byte isDeleted);
    }
}
