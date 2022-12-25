using Craftorio.Shared;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Http;
using Microsoft.AspNetCore.Components;
using System.Net;

namespace Craftorio.Server.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/login")]
    public class LoginController : ControllerBase
    {
        private readonly ISessionController _sessionController;
        private readonly ILogger<LoginController> _logger;
        public LoginController(ILogger<LoginController> logger, ISessionController sessionController)
        {
            _logger = logger;
            _sessionController = sessionController;
        }
        [HttpGet]
        public string Get()
        {
            return "haha";
        }
        [HttpPost]
        public ActionResult Post(Credentials credentials)
        {
            try
            {
                LoginDbConnector connector = new LoginDbConnector();
                if (connector.Match(credentials))
                {
                    System.Net.Cookie sessionCookie = _sessionController.Create(credentials);
                    Response.Cookies.Append(sessionCookie.Name, sessionCookie.Value);
                    return Content(sessionCookie.Value);
                }
                else
                {
                    //Unauth
                    return StatusCode(401);
                }
            }catch(Session.DuplicateSessionException dupSessEx)
            {
                return StatusCode(409,"User already logged in, please log out from different computer.");
            }
        }
    }
}