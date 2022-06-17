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
                Audience = "TestAudience",
                Issuer = "TestIssuer",
                JWTKey = "&3h32SlghVhd284hfs"
            };

            _service = new TokenService(config);
        }



        [Fact]
        public void Valid_Token()
        {
            var claims = new List<Claim>();

            var newToken = _service.IssueTokenOfType("confirm account",TimeSpan.FromHours(24), claims);

            var result = _service.ValidateTokenOfType("confirm account", newToken.Value);

            Assert.Equal(ResultStatus.Success, result.Status);


        }

        [Fact]
        public void Invalid_Token_Wrong_Type()
        {
            var claims = new List<Claim>();

            var newToken = _service.IssueTokenOfType("confirm account", TimeSpan.FromHours(24), claims);

            var result = _service.ValidateTokenOfType("WrongType", newToken.Value);

            Assert.Equal(ResultStatus.Tk_Invalid, result.Status);


        }

        [Fact]
        public void Invalid_Token_Claim_Value_Mismatch()
        {
            var inClaims = new List<Claim>()
            {
                new Claim("A_TYPE","Value1")
            };
            var outClaims = new List<Claim>()
            {
                new Claim("A_TYPE","Value2")
            };



            var newToken = _service.IssueTokenOfType("confirm account", TimeSpan.FromHours(24), inClaims);

            var result = _service.ValidateTokenOfType("confirm account", newToken.Value,outClaims);

            Assert.Equal(ResultStatus.Tk_Invalid, result.Status);


        }


        [Fact]
        public void Using_Test()
        {
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


                var validateResult = service.ValidateTokenOfType("my custom claim type", newToken.Value, claims);
            }
        }
    }
}