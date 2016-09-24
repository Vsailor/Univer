using COM.Chat.Client.Models;
using System.Threading.Tasks;

namespace COM.Chat.Client.Presentation.Services.Abstract
{
    interface IRegisterService
    {
        Task RegisterUser(string login, string password);
        Task<User> GetUserByLogin(string login);
    }
}
