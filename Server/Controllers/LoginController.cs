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
        [HttpPost]
        public ActionResult Post(Credentials credentials)
        {
            try
            {
                LoginDbConnector connector = new LoginDbConnector();
                if (connector.Match(credentials))
                {
                    return Content(_sessionController.Create(credentials));
                }
                else
                {
                    //Unauth
                    return StatusCode(401);
                }
            }catch(DuplicateSessionException dupSessEx)
            {
                return StatusCode(409,"User already logged in, please log out from different computer.");
            }
        }
    }
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/logout")]
    public class LogoutController : ControllerBase
    {
        private readonly ISessionController _sessionController;
        public LogoutController(ISessionController sessionController)
        {
            _sessionController = sessionController;
        }
        /// <summary>
        /// Called when user wants to log out
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Post(Session session)
        {
            //Checks the session token
            try
            {
                _sessionController.IsLogged(session);
            }
            catch (ArgumentNullException ex)
            {
                //session is not logged, possibly someone messing with us?
                //TODO: catch random ppl abusing API
                return StatusCode(404);
            }
            //Logout the session
            _sessionController.Logout(session);
            return StatusCode(200);
        }
    }
}