---
OVERVIEW

---

Library provides a simple way to issue and validate Json Web Tokens of a given type with (optional) claims.

---
USAGE

Create a new IEmailService instance (manually or via DI with .ASPNETCore project). 'TokenConfig' is passed to the constructor and contains the Key,Issuer and Audience values along with options on how inbound tokens should be validated.

Tokens can be created and valiated by calling the 'IssueTokenOfType' and 'ValidateTokenOfType' Methods respectively.

---
CLASSES

---
TOKENCONFIG

---

        public string JWTKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public bool ValidateAudience { get; set; } // Default TRUE
        public bool ValidateIssuer { get; set; } // Default TRUE
        public bool ValidateExpiry { get; set; } // Default TRUE

---
TOKEN

---

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

---
METHODS

---

Method: IEmailService.IssueTokenOfType(string type, TimeSpan duration, ICollection<Claim> claims = null)

Parameters: type: The type of token to create | duration: how long the token should remain valid for | claims: optional additional claims to add

Returns: Token
  
Details: 

Takes token information creates JWT and returns an instance of Token class with token details.

Example:

            // This is created manually but can be done via DI using the
            // ASPNETCore Project

            var config = new TokenConfig()
            {
                Audience = "TestAudience",
                Issuer = "TestIssuer",
                JWTKey = "&3h32SlghVhd284hfs"
            };

            using (var service = new TokenService(config))
            {
                // Create any additional claims to include in the token
                List<Claim> claims = new List<Claim>()
                {
                    new Claim("MY_TYPE","my_type_value")
                };

                var newToken = service.IssueTokenOfType("my custom claim type",TimeSpan.FromHours(24), claims);
            }

---

Method: IEmailService.ValidateTokenOfType(string type, string tokenValue, ICollection<Claim> claims = null)

Parameters: type: The type of token to create | tokenValue: the string value of the token to validate | claims: list of claims which the given token MUST have.

Returns: Result instance with either Success OR Tk_Invalid status.
  
Details: 

Takes the token value and ensures the token can be read, is the correct type and (optionally) includes all of the provided claims.

Example:

            // This is created manually but can be done via DI using the
            // ASPNETCore Project

            var config = new TokenConfig()
            {
                Audience = "TestAudience",
                Issuer = "TestIssuer",
                JWTKey = "&3h32SlghVhd284hfs"
            };

            using (var service = new TokenService(config))
            {
                // Create any additional claims to include in the token
                List<Claim> claims = new List<Claim>()
                {
                    new Claim("MY_TYPE","my_type_value")
                };

                var newToken = service.IssueTokenOfType("my custom claim type",TimeSpan.FromHours(24), claims);
                
                // The above is the same for issuing a token...

                var validateResult = service.ValidateTokenOfType("my custom claim type", newToken.Value, claims);
            }

















