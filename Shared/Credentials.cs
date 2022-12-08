using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Craftorio.Shared
{
    public class Credentials
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public Credentials(string username, string password)
        {
            if(username == null || username == "")
            {
                throw new ArgumentException("Username must be filled.");
            }
            if(password == null || password == "")
            {
                throw new ArgumentException("Password must be filled");
            }
            this.Username = username;
            this.Password = password;
        }
        private Credentials()
        {
            this.Username = null;
            this.Password = null;
        }
        /// <summary>
        /// Returns <code>Credentials</code> with null username and password
        /// </summary>
        /// <returns></returns>
        public static Credentials NullCredentials()
        {
            return new Credentials();
        }
    }
}
