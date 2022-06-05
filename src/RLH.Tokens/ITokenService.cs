using RLH.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RLH.Tokens
{
    public interface ITokenService : IDisposable
    {
        /// <summary>
        /// Creates and returned a new token of type provided. Optional Claims can be included.
        /// </summary>
        /// <param name="type">Type of token to create</param>
        /// <param name="claims">(Optional) additional Claims</param>
        /// <returns>Generated Token class instance</returns>
        public Token IssueTokenOfType(TokenType type, ICollection<Claim> claims = null);

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
        public Result<bool> ValidateTokenOfType(TokenType type,string tokenValue,ICollection<Claim> claims = null );

    }
}
