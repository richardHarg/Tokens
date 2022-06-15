using System.Security.Claims;
using RLH.Results;

namespace RLH.Tokens.Tests
{
    public class TokenServiceTests
    {
        private readonly TokenService _service;
        public TokenServiceTests()
        {
            var config = new TokenConfig()
            {
                JWTDuration = TimeSpan.FromHours(48),
                GeneralDuration = TimeSpan.FromHours(24),
                Audience = "TestAudience",
                Issuer = "TestIssuer",
                GeneralKey = "65jdhasaskjGJd735231",
                JWTKey = "&3h32SlghVhd284hfs"
            };

            _service = new TokenService(config);
        }



        [Fact]
        public void Test1()
        {
            var claims = new List<Claim>();

            var newToken = _service.IssueTokenOfType(TokenType.CONFIRM_ACCOUNT, claims);

            var result = _service.ValidateTokenOfType(TokenType.CONFIRM_ACCOUNT,newToken.Value);

            Assert.Equal(Result.ResultStatus.Success, result.Status);


        }
    }
}