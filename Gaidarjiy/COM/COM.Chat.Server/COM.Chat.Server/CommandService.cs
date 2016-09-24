using System.Collections.Generic;

namespace COM.Chat.Server
{
    public static class CommandService
    {
        public static readonly Dictionary<SqlCommands, string> CommandDictionary = new Dictionary<SqlCommands, string>
        {
            { SqlCommands.InsertUser, "INSERT INTO [dbo].[User] ([Login],[Password],[IsDeleted]) VALUES ({0}, {1}, {2})" }
        };
    }

    public enum SqlCommands
    {
        InsertUser
    }
}
