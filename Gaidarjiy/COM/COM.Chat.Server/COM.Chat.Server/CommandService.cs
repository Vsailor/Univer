using System.Collections.Generic;

namespace COM.Chat.Server
{
    public static class CommandService
    {
        public static readonly Dictionary<SqlCommands, string> CommandDictionary = new Dictionary<SqlCommands, string>
        {
            { SqlCommands.InsertUser, "INSERT INTO [User] (Login, Password, IsDeleted) VALUES ({0}, {1}, {2})" },
            { SqlCommands.GetUserByLogin, "SELECT Login, Password, IsDeleted FROM [User] WHERE Login = {0} AND IsDeleted = {1}" },
            { SqlCommands.GetUsersLogins, "SELECT Login FROM [User]" },
            { SqlCommands.InsertMessage, "INSERT INTO Message (Sender_login, Receiver_login, [Content], Date, IsDeleted) VALUES ('{0}', '{1}', '{2}', '{3}', 0)" },
            { SqlCommands.UpdateUserMessageFlag, "UPDATE [User] SET HasIncomingMessage = {0} WHERE Login = '{1}'"},
            { SqlCommands.GetUserMessageFlag, "SELECT HasIncomingMessage FROM [User] WHERE Login = '{0}'"},
            { SqlCommands.GetUserDialog, "SELECT Sender_login, Receiver_login, [Content], [Date] FROM Message WHERE (Sender_login = '{0}' AND Receiver_login = '{1}') OR (Sender_login = '{1}' AND Receiver_login = '{0}') ORDER BY [Date]" }
        };
    }

    public enum SqlCommands
    {
        InsertUser,
        GetUserByLogin,
        GetUsersLogins,
        InsertMessage,
        UpdateUserMessageFlag,
        GetUserMessageFlag,
        GetUserDialog
    }
}
