using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace COM.Chat.Server.Abstract
{
    [ComVisible(true)]
    interface IMessageRepository
    {
        Task Insert(string connectionString, string content, string reciever, string sender, DateTime time);
    }
}
