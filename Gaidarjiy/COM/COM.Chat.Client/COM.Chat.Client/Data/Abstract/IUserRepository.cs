using System.Data.SqlClient;
using System.Threading.Tasks;

namespace COM.Chat.Client.Data.Abstract
{
    interface IUserRepository
    {
        Task RegisterUser(string login, string password, int isDeleted);
    }
}
