
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
        /// Timespace a given JWT should remain valid for
        /// </summary>
        public TimeSpan JWTDuration{ get; set; }
        /// <summary>
        /// The service/site which is issuing the token
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// The intended audience for the token
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// General Key used to generate none JWT's
        /// </summary>
        public string GeneralKey { get; set; }
        /// <summary>
        /// Timespace a given general token should remain valid for
        /// </summary>
        public TimeSpan GeneralDuration { get; set; }
    }
}
