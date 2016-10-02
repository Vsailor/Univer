using COM.Chat.Client.Data.Abstract;
using COM.Chat.Client.Models;
using System.Threading.Tasks;
using System.Reflection;

namespace COM.Chat.Client.Data
{
    public class MessageRepository : Repository, IMessageRepository
    {
        public MessageRepository() : base("COM.Chat.Server.MessageRepository") { }

        public Task InsertMessage(Message message)
        {
            MethodInfo insertMethodInfo = RepositoryCOMType.GetMethod("Insert");
            return (Task)insertMethodInfo.Invoke(
                RepositoryCOMObject, 
                new object[] {
                    ConnectionString,
                    message.Content,
                    message.ReceiverLogin,
                    message.SenderLogin,
                    message.Time
                });
        }
    }
}
