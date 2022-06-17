
namespace RLH.Tokens
{
    /// <summary>
    /// Options related to generating end user tokens
    /// </summary>
    public class TokenConfig
    {
        /// <summary>
        /// Key used to generate JWT Tokens
        /// </summary>
        public string JWTKey { get; set; }
        /// <summary>
        /// The service/site which is issuing the token
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// The intended audience for the token
        /// </summary>
        public string Audience { get; set; }

        public bool ValidateAudience { get; set; } = true;
        public bool ValidateIssuer { get; set; } = true;
        public bool ValidateExpiry { get; set; } = true;
    }
}
