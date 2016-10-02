using COM.Chat.Client.Models;
using COM.Chat.Client.Presentation.Services.Abstract;
using COM.Chat.Client.Services;
using COM.Chat.Client.Services.Abstract;
using System.Threading.Tasks;

namespace COM.Chat.Client.Presentation.Services
{
    public class MessagePresentationService : IMessagePresentationService
    {
        IMessageService _messageService;
        public MessagePresentationService()
        {
            _messageService = new MessageService();
        }

        public async Task SendMessage(Message message)
        {
            await _messageService.SendMessage(message);
        }
    }
}
