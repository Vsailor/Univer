using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace COM.Chat.Server.Abstract
{
    [ComVisible(true)]
    interface IUserRepository
    {
        Task Insert(string connectionString, string login, string password, int isDeleted);
    }
}
