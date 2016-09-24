using COM.Chat.Server.Abstract;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace COM.Chat.Server
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class UserRepository : IUserRepository
    {
        public async Task Insert(string connectionString, string login, string password, int isDeleted)
        {
            var command = new SqlCommand();
            command.CommandText = string.Format(CommandService.CommandDictionary[SqlCommands.InsertUser], login, password, isDeleted);

            using (var conn = new SqlConnection(connectionString))
            {
                command.Connection = conn;
                await conn.OpenAsync();
                await command.ExecuteNonQueryAsync();
            }
        }
    }
}
