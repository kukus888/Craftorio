using Craftorio.Shared;
using System.IO;
using System.Text.Json;

namespace Craftorio.Server.Controllers
{
    public interface ILoginDbConnector
    {
        public void Create(Credentials credentials);
    }
    /// <summary>
    /// Singleton, used to access the Login Database
    /// </summary>
    public class LoginDbConnector:ILoginDbConnector
    {
        private readonly string loginDbPath = "./logins.json";
        protected LoginDbConnector Instance { get; }
        private List<Credentials> localCredentials { get; }
        public LoginDbConnector()
        {
            if(Instance == null)
            {
                Instance = this;
                localCredentials = new List<Credentials>();
                UpdateLocalMemory();
            }
            else
            {
                //Singleton has already been created
            }
        }
        /// <summary>
        /// Creates new user
        /// </summary>
        /// <param name="credentials"></param>
        public void Create(Credentials credentials)
        {
            localCredentials.Add(credentials);
            string serializedLogins = JsonSerializer.Serialize<Credentials[]>(localCredentials.ToArray());
            TextWriter tw = new StreamWriter(loginDbPath);
            tw.Write(serializedLogins);
            tw.Close();
            tw.Dispose();
        }
        /// <summary>
        /// Looks for the credentials in the database
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns><code>true</code> if credentials are in DB</returns>
        public bool Match(Credentials credentials)
        {
            Credentials c = localCredentials.Find(x => (x.Username == credentials.Username && x.Password == credentials.Password));
            if(c == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        private void Read()
        {
            throw new NotImplementedException();
        }
        private void Update()
        {
            throw new NotImplementedException();
        }
        public void Delete()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Reads the file with logins and usernames, and updates the local storage
        /// </summary>
        private void UpdateLocalMemory()
        {
            FileStream fs = File.OpenRead(loginDbPath);
            Credentials[] storedCredentials = JsonSerializer.Deserialize<Credentials[]>(fs);
            fs.Close();
            fs.DisposeAsync();
            localCredentials.Clear();
            foreach(Credentials c in storedCredentials)
            {
                localCredentials.Add(c);
            }
        }
        public bool UsernameExists(string username)
        {
            if (localCredentials.Exists(x => x.Username == username)) { return true; } else return false;
        }
    }
}
