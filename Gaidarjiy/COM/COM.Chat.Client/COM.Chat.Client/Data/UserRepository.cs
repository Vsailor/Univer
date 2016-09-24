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
    public class UserRepository : IUserRepository
    {
        private string _connectionString;
        private object _userRepositoryCOMObject;
        private Type _userRepositoryCOMType;

        public UserRepository()
        {
            _connectionString = ConfigurationManager.AppSettings["connectionString"];
            _userRepositoryCOMType = Type.GetTypeFromProgID("COM.Chat.Server.UserRepository");
            _userRepositoryCOMObject = Activator.CreateInstance(_userRepositoryCOMType);
        }

        public Task RegisterUser(string login, string password, byte isDeleted)
        {
            MethodInfo insertUserMethod = _userRepositoryCOMType.GetMethod("Insert");
            return (Task)(insertUserMethod.Invoke(_userRepositoryCOMObject, new object[] { _connectionString, login, password, isDeleted }));
        }

        public List<User> GetUserByLogin(string login, byte isDeleted)
        {
            MethodInfo selectMethodInfo = _userRepositoryCOMType.GetMethod("GetByLogin");
            object result = selectMethodInfo.Invoke(_userRepositoryCOMObject, new object[] { _connectionString, login, isDeleted });
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
    }
}
