using COM.Chat.Client.Models;
using System.Threading.Tasks;

namespace COM.Chat.Client.Presentation.Services.Abstract
{
    interface IMessagePresentationService
    {
        Task SendMessage(Message message);
    }
}
