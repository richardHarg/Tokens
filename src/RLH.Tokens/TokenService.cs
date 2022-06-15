
using Microsoft.IdentityModel.Tokens;
using RLH.Results;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RLH.Tokens
{
    public class TokenService : ITokenService
    {
        protected readonly TokenConfig _config;
        private bool disposedValue;

        /// <summary>
        /// Default constructor, passing a TokenConfig instance to configure the class
        /// </summary>
        /// <param name="config">Instance of TokenConfig</param>
        public TokenService(TokenConfig config)
        {
            _config = config;
        }

        /// <summary>
        /// Creates and returned a new token of type provided. Optional Claims can be included.
        /// </summary>
        /// <param name="type">Type of token to create</param>
        /// <param name="claims">(Optional) additional Claims</param>
        /// <returns>Generated Token class instance</returns>
        public Token IssueTokenOfType(TokenType type, ICollection<Claim> claims = null)
        {
            // Ensure the Type of token is added to the existing claims collection
            claims = AddTypeClaim(type, claims);

            // Create and return the generated token
            return IssueToken(type, claims);
        }

        /// <summary>
        /// Validates a provided token of a given type, IF optional claims were included when creating the token
        /// these should also be provided to ensure validation passes.
        /// NOTE: This method will validate a JWT however it should not be considered a replacement to built in/custom
        /// validators which exist as part of the ASPNETcore Web API pipeline.
        /// </summary>
        /// <param name="type">Type of token to create</param>
        /// <param name="tokenValue">The string value of the token</param>
        /// <param name="claims">(Optional) additional Claims to validate (should match those passed when creating the token)</param>
        /// <returns>Result class instance</returns>
        public Result ValidateTokenOfType(TokenType type, string tokenValue, ICollection<Claim> claims = null)
        {

            var handler = new JwtSecurityTokenHandler();

            // Ensure the Type of token is added to the existing claims collection
            claims = AddTypeClaim(type, claims);

            // Check if the string is formatted correctly and can be read by the handler, if this fails return error now
            if (handler.CanReadToken(tokenValue) == false)
            {
                return Result.InvalidToken("token", $"Provided token is unable to read");
            }
           
            // Perform validation and return a list of ValidationErrors, if any
            var validationErrors = ValidateToken(tokenValue, claims);

            // If there are any errors generated from the above checks return the correct Result status & pass errors
            if (validationErrors.Any())
            {
                return Result.InvalidToken(validationErrors.ToList());
            }

            // If not return Success!
            return Result.Success();
        }

        /// <summary>
        /// Validates a given token string with standard checks and additional Claims
        /// </summary>
        /// <param name="tokenValue">The string value of the token</param>
        /// <param name="claims">Additional Claims to validate (should match those passed when creating the token)</param>
        /// <returns>IEnumerable of ValidationError/s</returns>
        private IEnumerable<ValidationError> ValidateToken(string tokenValue, ICollection<Claim> claims)
        {
            // Read the token from the passed tokenValue string
            var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenValue);

            // Create a new list of validation errors to hold any errors encountered below
            var validationErrors = new List<ValidationError>();

            // Check the audience is valid
            if (token.Audiences.FirstOrDefault() != _config.Audience)
            {
                validationErrors.Add(new ValidationError("Audience", $"Configured audience '{_config.Audience}' does not match that provided by the token"));
            }
            // Check the issuer is valid
            if (token.Issuer != _config.Issuer)
            {
                validationErrors.Add(new ValidationError("Issuer", $"Configured issuer '{_config.Issuer}' does not match that provided by the token"));
            }
            // Check the token hasnt expired
            if (token.ValidTo < DateTimeOffset.UtcNow)
            {
                validationErrors.Add(new ValidationError("ValidTo", $"Token has expired"));

            }
            // Check the token type matches & any other Claims provided
            foreach (Claim claim in claims)
            {
                // Attempt to locate the token claim based on the Type value
                var tokenClaim = token.Claims.FirstOrDefault(x => x.Type == claim.Type);

                // if the claim CANNOT be found OR does NOT match that provided to this method then add error
                if (tokenClaim == null || tokenClaim.Value != claim.Value)
                {
                    validationErrors.Add(new ValidationError(claim.Type, $"Claim '{claim.Type}/{claim.Value}' is missing or invalid in provided token"));
                }

                // NOTE: Should there be futher validation here? audience etc. ?
            }

            return validationErrors;
        }


        /// <summary>
        /// Creates and issues a JWT Token contained in a Token class instance
        /// </summary>
        /// <param name="type">Type of token to create</param>
        /// <param name="claims">Additional Claims to validate (should match those passed when creating the token)</param>
        /// <returns></returns>
        private Token IssueToken(TokenType type, ICollection<Claim> claims)
        {
            // Select the key & duration to use based on the type provided
            string keyToUse;
            TimeSpan duration;

            switch (type)
            {
                case TokenType.JWT:
                    keyToUse = _config.JWTKey;
                    duration = _config.JWTDuration;
                    break;
                default:
                    keyToUse = _config.GeneralKey;
                    duration = _config.GeneralDuration;
                    break;
            }

            var token = new JwtSecurityToken(_config.Issuer,
                                             _config.Audience,
                                             claims,
                                             null,
                                             DateTime.UtcNow.AddHours(duration.TotalHours),
                                             new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(keyToUse)),
                                             SecurityAlgorithms.HmacSha256Signature
                                             ));

        
            return new Token(type,new JwtSecurityTokenHandler().WriteToken(token),DateTimeOffset.Now,duration);
        }

        /// <summary>
        /// Ensures that the TokenType is added as a claim to the provided collection
        /// If collection is NULL will create new then add TokenType claim
        /// </summary>
        /// <param name="type">Type of token to create</param>
        /// <param name="claims">Additional Claims to validate (should match those passed when creating the token)</param>
        /// <returns>ICollection of Claims</returns>
        private ICollection<Claim> AddTypeClaim(TokenType type,ICollection<Claim> claims)
        {
            // If NO optional claims were passed by the user create a new collection, add the Type claim and pass back
            if (claims == null)
            {
                return new List<Claim>()
                {
                    new Claim("Type",type.ToString())
                };
            }
            // If some claims were passed then add the Type claim and pass back
            else
            {
                claims.Add(new Claim("Type",type.ToString()));
                return claims;
            }
        }









        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TokenService()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

       
    }
}
