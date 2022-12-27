using Craftorio.Shared;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.Extensions.Http;
using Microsoft.AspNetCore.Components;
using System.Net;
using System.Text.Json;
using System.Xml.Serialization;

namespace Craftorio.Server.Controllers
{
    [ApiController]
    [Microsoft.AspNetCore.Mvc.Route("api/")]
    public class APIcontroller : ControllerBase
    {
        private readonly ISessionController _sessionController;
        private readonly IPlayerController _playerController;
        private readonly ILogger<APIcontroller> _logger;
        public APIcontroller(ILogger<APIcontroller> logger, ISessionController sessionController, IPlayerController playerController)
        {
            _logger = logger;
            _sessionController = sessionController;
            _playerController = playerController;
        }
        [HttpPost("login")]
        public ActionResult Post(Credentials credentials)
        {
            try
            {
                LoginDbConnector connector = new LoginDbConnector();
                if (connector.Match(credentials))
                {
                    //register session
                    string sessionToken = _sessionController.Create(credentials);
                    //register player
                    _playerController.RegisterPlayer(new Player(credentials.Username));
                    return Content(sessionToken);
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
        /// <summary>
        /// Called when user wants to log out
        /// </summary>
        /// <returns></returns>
        [HttpPost("logout")]
        public ActionResult PostLogout([FromBody] string s)
        {
            Session session = new Session(s.Split(':')[0], s.Split(':')[1]);
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
        [HttpPost("verify")]
        public ActionResult PostVerify([FromBody] string s)
        {
            Session session = new Session(s.Split(':')[0], s.Split(':')[1]);
            if (_sessionController.IsLogged(session))
            {
                return StatusCode(200);
            }
            else
            {
                //Unauthorized
                return StatusCode(401);
            }
        }
        [HttpPost("getPlayer")]
        public ActionResult PostGetPlayer([FromBody]string username)
        {
            Player p = null;
            try
            {
                p = _playerController.GetPlayer(username);
            }
            catch(ArgumentNullException ex)
            {
                return StatusCode(404);
            }
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(Player));
            TextWriter s = new StreamWriter("playerStream.xml");
            xmlSerializer.Serialize(s, p);
            s.Close();
            string[] playerStringifiedArr = System.IO.File.ReadAllLines("playerStream.xml");
            string playerStringified = "";
            for (int i = 0;i < playerStringifiedArr.Length;i++)
            {
                playerStringified += playerStringifiedArr[i];
            }
            return Ok(playerStringified);
        }
    }
}