using COM.Chat.Server.Abstract;
using COM.Chat.Server.Models;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace COM.Chat.Server
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class UserRepository : IUserRepository
    {
        public async Task Insert(string connectionString, string login, string password, byte isDeleted)
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

        public object GetByLogin(string connectionString, string login, byte isDeleted)
        {
            var command = new SqlCommand();
            command.CommandText = string.Format(CommandService.CommandDictionary[SqlCommands.GetUserByLogin], login, isDeleted);

            SqlDataReader reader;
            var users = new List<User>();

            using (var conn = new SqlConnection(connectionString))
            {
                command.Connection = conn;
                conn.Open();
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var user = new User()
                        {
                            Login = reader.GetString(0),
                            Password = reader.GetString(1),
                            IsDeleted = reader.GetBoolean(2)
                        };

                        users.Add(user);
                    }
                }

                reader.Close();
            }

            XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, users);
                return writer.ToString();
            }
        }
    }
}
