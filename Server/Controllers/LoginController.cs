using Craftorio.Shared;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Http;

namespace Craftorio.Server.Controllers
{
    [ApiController]
    [Route("api/login")]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;
        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public string Get()
        {
            return "haha";
        }
        [HttpPost]
        public ActionResult Post(Credentials credentials)
        {
            LoginDbConnector connector = new LoginDbConnector();
            if (connector.Match(credentials))
            {
                SessionController sessionController = new SessionController();
                System.Net.Cookie sessionCookie = sessionController.Create(credentials);
                Response.Cookies.Append(sessionCookie.Name, sessionCookie.Value);
                return Content("OK");
            }
            else
            {
                return Content("NOK");
            }
        }
    }
}