using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RLH.Tokens
{
    public sealed class Token
    {
        /// <summary>
        /// Internal constructor used to create a new Token instance
        /// </summary>
        /// <param name="type">Type of token</param>
        /// <param name="value">string value of the token</param>
        /// <param name="created">DatetimeOffset UTC timestamp when the token was created</param>
        /// <param name="expiresIn">Timespan representing how long the token will remain valid</param>
        internal Token(string type, string value,DateTimeOffset created,TimeSpan expiresIn)
        {
            Type = type;
            Value = value;
            Created = created;
            ExpiresIn = expiresIn;
        }

        /// <summary>
        /// Type of the token
        /// </summary>
        public readonly string Type;
        /// <summary>
        /// DatetimeOffset UTC when the token was created
        /// </summary>
        public readonly DateTimeOffset Created;
        /// <summary>
        /// String value of the token
        /// </summary>
        public readonly string Value;
        /// <summary>
        /// Timespan representing how long the token will remain valid
        /// </summary>
        public readonly TimeSpan ExpiresIn;

    }
}
