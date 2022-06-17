
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
        /// <param name="type">Name of the type of token to create, when validating this MUST match the name you pass here</param>
        /// <param name="claims">(Optional) additional Claims</param>
        /// <returns>Generated Token class instance</returns>
        public Token IssueTokenOfType(string type, TimeSpan duration, ICollection<Claim> claims = null)
        {
            // Ensure the provided token type name is consistant 
            type = NormaliseTokenTypeName(type);

            // Ensure the Type of token is added to the existing claims collection
            claims = SetupClaimList(type, claims);

            // Create and return the generated token
            return IssueToken(type, claims,duration);
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
        public Result ValidateTokenOfType(string type, string tokenValue, ICollection<Claim> claims = null)
        {
            type = NormaliseTokenTypeName(type);

            var handler = new JwtSecurityTokenHandler();

            // Ensure the Type of token is added to the existing claims collection
            claims = SetupClaimList(type, claims);

            // Check if the string is formatted correctly and can be read by the handler, if this fails return error now
            if (handler.CanReadToken(tokenValue) == false)
            {
                return Result.InvalidToken("token", $"Provided token is unable to be read");
            }
           
            // Perform validation and return a list of ValidationErrors, if any
            return ValidateToken(tokenValue, claims);
        }

        /// <summary>
        /// Validates a given token string with standard checks and additional Claims
        /// </summary>
        /// <param name="tokenValue">The string value of the token</param>
        /// <param name="claims">Additional Claims to validate (should match those passed when creating the token)</param>
        /// <returns>IEnumerable of ValidationError/s</returns>
        private Result ValidateToken(string tokenValue, ICollection<Claim> claims)
        {
            // Read the token from the passed tokenValue string
            var token = new JwtSecurityTokenHandler().ReadJwtToken(tokenValue);

            // Create a new list of validation errors to hold any errors encountered below
            var validationErrors = new List<ValidationError>();

            // Perform general token validations (audience,issuer and expiry)
            ValidateToken(token, validationErrors);

            // Validate the provided claims V. those extracted from the token
            ValidateClaims(token, validationErrors, claims);

            // If there are any errors generated from the above checks return the correct Result status & pass errors
            if (validationErrors.Any())
            {
                return Result.InvalidToken(validationErrors.ToList());
            }

            // If not return Success!
            return Result.Success();
        }


        /// <summary>
        /// Creates and issues a JWT Token contained in a Token class instance
        /// </summary>
        /// <param name="type">Type of token to create</param>
        /// <param name="claims">Additional Claims to validate (should match those passed when creating the token)</param>
        /// <returns></returns>
        private Token IssueToken(string type, ICollection<Claim> claims, TimeSpan duration)
        {
            var token = new JwtSecurityToken(_config.Issuer,
                                             _config.Audience,
                                             claims,
                                             null,
                                             DateTime.UtcNow.AddHours(duration.TotalHours),
                                             new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config.JWTKey)),
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
        private ICollection<Claim> SetupClaimList(string type,ICollection<Claim> claims)
        {
            // Ensure claims isnt null
            if (claims == null)
            {
                claims = new List<Claim>();
            }

            // Check and ensure a identical claim doesnt exist in the collection
            var existingClaim = claims.FirstOrDefault(x => x.Type == "RLH_TOKEN_CLAIM");

            if (existingClaim != null)
            {
                claims.Remove(existingClaim);
            }

            // Add the Token Claim
            claims.Add(new Claim("RLH_TOKEN_CLAIM", type));

            return claims;
        }

        /// <summary>
        /// Ensure the token name is standardised across creation/validation.
        /// Will convert to uppercase an replace spaces with _ char
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        private string NormaliseTokenTypeName(string value)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return value.ToUpper().Replace(' ', '_');
        }

       
        /// <summary>
        /// Performs general Token validation and, if required
        /// populates the provided collection of errors.
        /// </summary>
        /// <param name="token">Token details</param>
        /// <param name="validationErrors">ICollection of ValidationErrors</param>
        private void ValidateToken(JwtSecurityToken token, ICollection<ValidationError> validationErrors)
        {
            // Check the audience is valid
            if (_config.ValidateAudience == true && token.Audiences.FirstOrDefault() != _config.Audience)
            {
                validationErrors.Add(new ValidationError("Audience", $"Configured audience '{_config.Audience}' does not match that provided by the token"));
            }
            // Check the issuer is valid
            if (_config.ValidateIssuer == true && token.Issuer != _config.Issuer)
            {
                validationErrors.Add(new ValidationError("Issuer", $"Configured issuer '{_config.Issuer}' does not match that provided by the token"));
            }
            // Check the token hasnt expired
            if (_config.ValidateExpiry == true && token.ValidTo < DateTimeOffset.UtcNow)
            {
                validationErrors.Add(new ValidationError("ValidTo", $"Token has expired"));
            }
        }


        /// <summary>
        /// Performs specific validation on the claims provided Vs. the ones extracted from the token
        /// </summary>
        /// <param name="token">Token details</param>
        /// <param name="validationErrors">ICollection of ValidationErrors</param>
        /// <param name="claims">Claims to be checked against those from the token</param>
        private void ValidateClaims(JwtSecurityToken token, ICollection<ValidationError> validationErrors,ICollection<Claim> claims)
        {
            // Check the token type matches & any other Claims provided
            foreach (Claim claimFromProvidedList in claims)
            {
                // Attempt to locate the token claim based on the Type value
                var claimFromToken = token.Claims.FirstOrDefault(x => x.Type == claimFromProvidedList.Type);

                if (ValidateClaimSimple(claimFromToken,claimFromProvidedList) == false)
                {
                    validationErrors.Add(new ValidationError(claimFromProvidedList.Type, $"Claim '{claimFromProvidedList.Type}/{claimFromProvidedList.Value}' is missing or invalid in provided token"));

                }
            }
        }

        /// <summary>
        /// Ensures the Type of claim is present within the read token AND the value matches that provided 
        /// when calling the validate token method.
        /// </summary>
        /// <param name="claimFromToken"></param>
        /// <param name="claimFromProvidedList"></param>
        /// <returns></returns>
        private bool ValidateClaimSimple(Claim claimFromToken,Claim claimFromProvidedList)
        {
            // A reference claim with the given Type must exist
            if (claimFromToken == null)
            {
                return false;
            }
            // Check the value of the provided claim matches the one in the token
            if (claimFromToken.Value != claimFromProvidedList.Value)
            {
                return false;
            }

            return true;
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
