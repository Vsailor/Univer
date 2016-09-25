using COM.Chat.Client.Data.Abstract;
using System.Threading.Tasks;
using System;
using System.Reflection;
using System.Configuration;
using System.Collections.Generic;
using COM.Chat.Client.Models;
using System.Xml.Serialization;
using System.IO;

namespace COM.Chat.Client.Data
{
    public class UserRepository : Repository, IUserRepository
    {
        public UserRepository() : base("COM.Chat.Server.UserRepository") { }

        public Task RegisterUser(string login, string password, byte isDeleted)
        {
            MethodInfo insertUserMethod = RepositoryCOMType.GetMethod("Insert");
            return (Task)insertUserMethod.Invoke(RepositoryCOMObject, new object[] { ConnectionString, login, password, isDeleted });
        }

        public List<User> GetUserByLogin(string login, byte isDeleted)
        {
            MethodInfo selectMethodInfo = RepositoryCOMType.GetMethod("GetByLogin");
            object result = selectMethodInfo.Invoke(RepositoryCOMObject, new object[] { ConnectionString, login, isDeleted });
            var res = result.ToString();

            XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
            var users = new List<User>();
            using (var reader = new StringReader(res))
            {
                users = (List<User>)serializer.Deserialize(reader);
                reader.Close();
            }

            return users;
        }

        public List<string> GetUsersLogins()
        {
            MethodInfo selectMethodInfo = RepositoryCOMType.GetMethod("GetUsersLogins");
            object result = selectMethodInfo.Invoke(RepositoryCOMObject, new object[] { ConnectionString });
            var res = result.ToString();

            XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
            var users = new List<string>();
            using (var reader = new StringReader(res))
            {
                users = (List<string>)serializer.Deserialize(reader);
                reader.Close();
            }

            return users;
        }
    }
}
