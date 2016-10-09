using EasyScreenshot.WebApp.Services;
using EasyScreenshot.WebApp.View.Models;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace EasyScreenshot.WebApp.View.Controllers
{
    [RoutePrefix("api/connections")]
    public class ConnectionsController : ApiController
    {
        private ConnectionService _connectionService = new ConnectionService();

        public string Get(int id)
        {
            var conn = _connectionService.TakeIncomingConnection(id);
            if (conn == null)
                return string.Empty;

            return $@"url={conn.URL}&from={conn.FromId}&to={conn.ToId}";
        }

        [HttpGet]
        [Route("create/{fromId}/{toId}/{url}")]
        public HttpStatusCode Get(int fromId, int toId, string url)
        {
            var connection = new ConnectionVM
            {
                FromId = fromId,
                ToId = toId,
                URL = url
            };

            return _connectionService.CreateConnection(connection);
        }

        [HttpGet]
        [Route("register")]
        public int Register()
        {
            return _connectionService.RegisterUser();
        }

        [HttpGet]
        [Route("unregister/{userId}")]
        public async Task<HttpStatusCode> Unregister(int userId)
        { 
            return await _connectionService.Unregister(userId);
        }

        [HttpGet]
        [Route("users/{userId}")]
        public HttpStatusCode GetUser(int userId)
        {
            if (_connectionService.IsUserExists(userId))
            {
                return HttpStatusCode.OK;
            }

            return HttpStatusCode.NotFound;
        }
    }
}
