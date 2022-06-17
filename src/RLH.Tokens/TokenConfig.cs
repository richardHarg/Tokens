
namespace RLH.Tokens
{
    /// <summary>
    /// Options related to generating end user tokens
    /// </summary>
    public class TokenConfig
    {
        public string JWTKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public bool ValidateAudience { get; set; } = true;
        public bool ValidateIssuer { get; set; } = true;
        public bool ValidateExpiry { get; set; } = true;
    }
}
