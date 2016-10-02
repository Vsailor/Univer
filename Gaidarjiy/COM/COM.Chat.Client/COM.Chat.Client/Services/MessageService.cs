using System;
using System.Threading.Tasks;
using COM.Chat.Client.Models;
using COM.Chat.Client.Services.Abstract;
using COM.Chat.Client.Data.Abstract;
using COM.Chat.Client.Data;

namespace COM.Chat.Client.Services
{
    public class MessageService : IMessageService
    {
        private IMessageRepository _messageRepository;

        public MessageService()
        {
            _messageRepository = new MessageRepository();
        }

        public async Task SendMessage(Message message)
        {
            await _messageRepository.InsertMessage(message);
        }
    }
}
