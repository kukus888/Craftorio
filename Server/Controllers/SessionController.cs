using Craftorio.Shared;
using System.Net;
using System.Security.Cryptography;

namespace Craftorio.Server.Controllers
{
    public interface ISessionController
    {
        /// <summary>
        /// Creates a session, and returns a sessionToken
        /// </summary>
        /// <param name="credentials">Credentials of the user</param>
        /// <returns>sessionToken</returns>
        string Create(Credentials credentials);
        /// <summary>
        /// Checks whether the session is active
        /// </summary>
        /// <param name="session">session of a user</param>
        /// <returns>Bool whether user is logged</returns>
        /// <exception cref="ArgumentNullException"></exception>
        bool IsLogged(Session session);
        bool IsLogged(string sessionToken, string username);
        /// <summary>
        /// Logs out the session
        /// </summary>
        /// <param name="session"></param>
        /// <exception cref="InvalidDataException">Invalid Session</exception>
        void Logout(Session session);
    }
    /// <summary>
    /// Singleton responsible for session management
    /// </summary>
    public class SessionController : ISessionController
    {
        private readonly ILogger<SessionController> logger;
        protected SessionController Instance { get; }
        private List<Session> sessionList { get; }
        public SessionController(ILogger<SessionController> logger)
        {
            if (Instance == null)
            {
                Instance = this;
                sessionList = new List<Session>();
            }
            else
            {
                //Singleton has already been created
            }

            this.logger = logger;
        }
        public string Create(Credentials credentials)
        {
            //byte[] bytes = new byte
            SHA256 sha256 = new SHA256Managed();
            byte[] hash = sha256.ComputeHash(ByteAdapter.StringToByteArray(credentials.Username+credentials.Password));
            //convert gibberish hash numbers into displayable ascii text
            for(int i = 0;i< hash.Length; i++)
            {
                if (hash[i] <= 47)
                {
                    hash[i] = (byte)(hash[i] + 49);
                }
                if (hash[i] >= 58 && hash[i] <= 64)
                {
                    hash[i] = (byte)(hash[i] - 10);
                }
                if (hash[i] >= 91 && hash[i] <= 96)
                {
                    hash[i] = (byte)(hash[i] - 10);
                }
                if (hash[i] >= 123)
                {
                    hash[i] = (byte)(hash[i]-65);
                    i--;//pass checks once again
                }
            }
            string shash = ByteAdapter.ByteArrayToString(hash);
            Session session = new Session(credentials.Username, shash);
            //Check for Session collisions
            foreach(Session s in sessionList)
            {
                if (s.ToString() == session.ToString())
                {
                    throw new DuplicateSessionException("Cannot initialize more than one session.");
                }
            }
            sessionList.Add(session);
            logger.Log(LogLevel.Debug, new EventId(), $"Logged in: {credentials.Username}");
            return shash;
        }
        public void Logout(Session s)
        {
            if(sessionList.Contains<Session>(s))
            {
                sessionList.Remove(s);
                logger.Log(LogLevel.Debug, new EventId(), $"Logged out: {s.username}");
            }
            else
            {
                InvalidSessionException ex = new InvalidSessionException("Session does not exist, hence cannot be removed!");
                logger.LogCritical(ex, $"Invalid logout: {s.sessionToken}");
                throw ex;
            }
        }
        /// <summary>
        /// Checks whether the session is active
        /// </summary>
        /// <param name="session">session of a user</param>
        /// <returns>Bool whether user is logged</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public bool IsLogged(Session session)
        {
            return sessionList.Contains<Session>(session);
        }
        public bool IsLogged(string sessionToken, string username)
        {
            return sessionList.Contains<Session>(new Session(username, sessionToken));
        }
    }
    public static class ByteAdapter
    {
        public static byte[] StringToByteArray(string s)
        {
            List<byte> bytes = new List<byte>();
            foreach (char c in s)
            {
                bytes.Add((byte)c);
            }
            return bytes.ToArray();
        }
        public static string ByteArrayToString(byte[] byteArray)
        {
            string s = "";
            foreach(byte b in byteArray)
            {
                s += (char)b;
            }
            return s;
        }
    }
}
