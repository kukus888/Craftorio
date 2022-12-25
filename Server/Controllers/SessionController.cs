using Craftorio.Shared;
using System.Net;
using System.Security.Cryptography;

namespace Craftorio.Server.Controllers
{
    public interface ISessionController
    {
        Cookie Create(Credentials credentials);
        bool IsLogged(string username);
    }
    /// <summary>
    /// Singleton responsible for session management
    /// </summary>
    public class SessionController : ISessionController
    {
        protected SessionController Instance { get; }
        private List<Session> sessionList { get; }
        public SessionController()
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
        }
        public Cookie Create(Credentials credentials)
        {
            //byte[] bytes = new byte
            SHA256 sha256 = new SHA256Managed();
            byte[] hash = sha256.ComputeHash(ByteAdapter.StringToByteArray(credentials.Username+credentials.Password));
            //convert gibberish hash numbers into displayable ascii text
            for(int i = 0;i< hash.Length; i++)
            {
                if (hash[i] <= 32)
                {
                    hash[i] = (byte)(hash[i] + 33);
                }
                if (hash[i] >= 127)
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
                    throw new Session.DuplicateSessionException("Cannot initialize more than one session.");
                }
            }
            sessionList.Add(session);
            return session.sessionCookie;
        }
        public bool IsLogged(string username)
        {
            foreach(Session s in sessionList)
            {
                if(s.player.username == username)
                {
                    return true;
                }
            }
            return false;
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
