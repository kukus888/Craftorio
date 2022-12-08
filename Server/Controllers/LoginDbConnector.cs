using Craftorio.Shared;
using System.IO;
using System.Text.Json;

namespace Craftorio.Server.Controllers
{
    /// <summary>
    /// Singleton, used to access the Login Database
    /// </summary>
    public class LoginDbConnector
    {
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
            throw new NotImplementedException();
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
            FileStream fs = File.OpenRead("./logins.json");
            Credentials[] storedCredentials = JsonSerializer.Deserialize<Credentials[]>(fs);
            fs.Close();
            fs.DisposeAsync();
            localCredentials.Clear();
            foreach(Credentials c in storedCredentials)
            {
                localCredentials.Add(c);
            }
        }
    }
}
