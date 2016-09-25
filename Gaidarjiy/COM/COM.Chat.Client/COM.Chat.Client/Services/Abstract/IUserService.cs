using COM.Chat.Client.Models;
using System.Threading.Tasks;

namespace COM.Chat.Client.Services.Abstract
{
    public interface IUserService
    {
        Task RegisterUser(string login, string password);
        User GetUserByLogin(string login);
    }
}
