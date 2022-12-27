using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net;
using System.Text.Json.Serialization;

namespace Craftorio.Shared
{
    public class Session
    {
        /// <summary>
        /// identificator
        /// </summary>
        public string sessionToken { get; }
        public string username { get; }
        /// <summary>
        /// Creates a new session with given token
        /// </summary>
        /// <param name="_sessionToken"></param>
        public Session(string _username, string _sessionToken)
        {
            this.username = _username;
            this.sessionToken = _sessionToken;
        }
        [JsonConstructor]
        public Session() {
            this.username = null;
            this.sessionToken = null;
        }
        public override string ToString()
        {
            return $"{this.username}:{this.sessionToken}";
        }
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            Session s = obj as Session;
            if(s.username == this.username && s.sessionToken == this.sessionToken) { return true; } else { return false; }
            return base.Equals(obj);
        }
    }
    public class DuplicateSessionException : Exception
    {
        public DuplicateSessionException(string? message) : base(message) { }
    }
    public class InvalidSessionException : Exception
    {
        public InvalidSessionException(string? message) : base(message) { }
        public InvalidSessionException(string? message, Exception innerException) : base(message, innerException) { }
    }
}
