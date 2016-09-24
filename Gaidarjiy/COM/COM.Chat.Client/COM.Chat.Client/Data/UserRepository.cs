using COM.Chat.Client.Data.Abstract;
using System.Threading.Tasks;
using System;
using System.Reflection;

namespace COM.Chat.Client.Data
{
    public class UserRepository : IUserRepository
    {
        private const string ConnectionString = "Data Source=IVAN-LAPTOP\\SQLEXPRESS;Initial Catalog=RBD_Chat_DB;Integrated Security=True";

        public Task RegisterUser(string login, string password, int isDeleted)
        {
            Type type = Type.GetTypeFromProgID("COM.Chat.Server.UserRepository");
            var userRepository = Activator.CreateInstance(type);
            MethodInfo mMulty = type.GetMethod("Insert");
            return (Task)(mMulty.Invoke(userRepository, new object[] { ConnectionString, login, password, isDeleted }));
        }
    }
}
