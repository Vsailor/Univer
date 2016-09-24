using COM.Chat.Client.Data.Abstract;
using System.Threading.Tasks;
using System;
using System.Data.SqlClient;

namespace COM.Chat.Client.Data
{
    public class UserRepository : IUserRepository
    {
        private const string ConnectionString = "Data Source=IVAN-LAPTOP\\SQLEXPRESS;Initial Catalog=RBD_Chat_DB;Integrated Security=True";

        public async Task RegisterUser(string login, string password, int isDeleted)
        {
            SqlCommand command = new SqlCommand(
               $@"INSERT INTO [dbo].[User] ([Login],[Password],[IsDeleted]) VALUES ({login}, {password}, {isDeleted})");
            using (var conn = new SqlConnection(ConnectionString))
            {
                command.Connection = conn;
                await conn.OpenAsync();
                await command.ExecuteNonQueryAsync();
                conn.Close();
            }
        }
    }
}
