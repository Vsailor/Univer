﻿namespace COM.Chat.Server.Models
{
    public class User
    {
        public string Login { get; set; }
        public string Password { get; set; }
        public bool IsDeleted { get; set; }
    }
}
