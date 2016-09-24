using System.Collections.Generic;

namespace COM.Chat.Server
{
    public static class CommandService
    {
        public static readonly Dictionary<SqlCommands, string> CommandDictionary = new Dictionary<SqlCommands, string>
        {
            { SqlCommands.InsertUser, "INSERT INTO [User] (Login, Password, IsDeleted) VALUES ({0}, {1}, {2})" },
            { SqlCommands.GetUserByLogin, "SELECT Login, Password, IsDeleted FROM [User] WHERE Login = {0} AND IsDeleted = {1}" }
        };
    }

    public enum SqlCommands
    {
        InsertUser,
        GetUserByLogin
    }
}
