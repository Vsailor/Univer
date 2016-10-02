using COM.Chat.Client.Models;
using System.Threading.Tasks;

namespace COM.Chat.Client.Services.Abstract
{
    public interface IMessageService
    {
        Task SendMessage(Message message);
    }
}
