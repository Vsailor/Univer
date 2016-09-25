using COM.Chat.Server.Abstract;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System;

namespace COM.Chat.Server
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class MessageRepository : IMessageRepository
    {
        public async Task Insert(string connectionString, string content, string reciever, string sender, DateTime time)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                SqlCommand command = conn.CreateCommand();
                conn.Open();
                SqlTransaction tran = conn.BeginTransaction("Insert message transaction");

                try
                {
                    command.Connection = conn;
                    command.Transaction = tran;
                    command.CommandText = string.Format(CommandService.CommandDictionary[SqlCommands.InsertMessage], sender, reciever, content, time.ToString("yyyy-MM-dd hh:mm:ss"));
                    await command.ExecuteNonQueryAsync();

                    command.CommandText = string.Format(CommandService.CommandDictionary[SqlCommands.UpdateUserMessageFlag], 1, reciever);
                    await command.ExecuteNonQueryAsync();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                }
            }
        }
    }
}
