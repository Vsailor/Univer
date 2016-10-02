using COM.Chat.Client.Models;
using System.Threading.Tasks;

namespace COM.Chat.Client.Data.Abstract
{
    interface IMessageRepository
    {
        Task InsertMessage(Message message);
    }
}
