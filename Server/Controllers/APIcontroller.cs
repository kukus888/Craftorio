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
using System.Numerics;

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
                    Player p;
                    //register session
                    string sessionToken = _sessionController.Create(credentials);
                    //load player if has account
                    string[] savedGames = Directory.GetFiles("./SavedGames/");
                    if (savedGames.Contains("./SavedGames/" + credentials.Username))
                    {
                        //player has saved some things
                        TextReader textReader = new StreamReader("./SavedGames/" + credentials.Username);
                        XmlSerializer xmlSerializer = new XmlSerializer(typeof(Player));
                        p = (Player) xmlSerializer.Deserialize(textReader);
                        textReader.Close();
                        textReader.Dispose();
                    }
                    else
                    {
                        p = new Player(credentials.Username);
                    }
                    //register player
                    _playerController.RegisterPlayer(p);
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
        /// <summary>
        /// Gets player from the playerController
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
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
            MemoryStream stream = new MemoryStream();
            xmlSerializer.Serialize(stream, p);
            StreamReader streamReader = new StreamReader(stream);
            stream.Position = 0;
            string playerStringified = streamReader.ReadToEnd();
            return Ok(playerStringified);
        }
        [HttpPost("saveProgress")]
        public void PostSaveProgress([FromBody] string[] s)
        {
            //verify user
            Session session = new Session(s[0].Split(':')[0], s[0].Split(':')[1]);
            if (_sessionController.IsLogged(session))
            {
                //user verified, get player data, in s[1]
                MemoryStream stream = new MemoryStream();
                TextWriter textWriter = new StreamWriter("./SavedGames/"+session.username);
                textWriter.Write(s[1]);
                textWriter.Flush();
                textWriter.Dispose();
                stream.Dispose();
            }
            else
            {
                //something is fishy
            }
        }
        [HttpPut("keepAlive")]
        public void PutKeepAlive([FromBody] string s)
        {
            //verify user
            Session session = new Session(s.Split(':')[0], s.Split(':')[1]);
            if (_sessionController.IsLogged(session))
            {
                _sessionController.Ping(session);
                //user verified, keep him on the hook
            }
            else
            {
                //something is fishy, or player has lost connection for longer than 2m
            }
        }
    }
}