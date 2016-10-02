﻿using System;

namespace COM.Chat.Client.Models
{
    public class Message
    {
        public string SenderLogin { get; set; }
        public string ReceiverLogin { get; set; }
        public string Content { get; set; }
        public DateTime Time { get; set; }
    }
}
