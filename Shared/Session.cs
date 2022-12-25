using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;

namespace Craftorio.Shared
{
    public class Session
    {
        /// <summary>
        /// identificator used in cookie
        /// </summary>
        public string identificator { get; }
        public Cookie sessionCookie { get; }
        public sPlayer player { get; }
        /// <summary>
        /// Creates a new session with given ID
        /// </summary>
        /// <param name="id"></param>
        public Session(string username, string id)
        {
            this.identificator = id;
            this.sessionCookie = new Cookie("session", identificator);
            this.player = new sPlayer(username);
        }
        public override string ToString()
        {
            return $"{this.identificator}";
        }
        public class DuplicateSessionException : Exception
        {
            public string Message { get; set; }
            public DuplicateSessionException(string? message):base(){ this.Message = message; }
        }
    }
}
