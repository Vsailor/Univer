using System;
using System.Configuration;

namespace COM.Chat.Client.Data
{
    public class Repository
    {
        protected string ConnectionString { get; private set; }

        protected object RepositoryCOMObject { get; private set; }

        protected Type RepositoryCOMType { get; private set; }

        protected string ProgID { get; private set; }

        public Repository(string progId)
        {
            ProgID = progId;
            ConnectionString = ConfigurationManager.AppSettings["connectionString"];
            RepositoryCOMType = Type.GetTypeFromProgID(ProgID);
            RepositoryCOMObject = Activator.CreateInstance(RepositoryCOMType);
        }
    }
}
