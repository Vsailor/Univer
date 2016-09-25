using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace COM.Chat.Server.Abstract
{
    [ComVisible(true)]
    interface IUserRepository
    {
        Task Insert(string connectionString, string login, string password, byte isDeleted);
        object GetByLogin(string connectionString, string login, byte isDeleted);
        object GetUsersLogins(string connectionString);
    }
}
