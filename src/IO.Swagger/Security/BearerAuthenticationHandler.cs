using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;

namespace IO.Swagger.Security
{
    /// <summary>
    /// class to handle bearer authentication.
    /// </summary>
    public class BearerAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        /// <summary>
        /// scheme name for authentication handler.
        /// </summary>
        public const string SchemeName = "Bearer";
        public IConfiguration _configuration { get; private set; }

        public BearerAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock, IConfiguration configuration) : base(options, logger, encoder, clock)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// verify that require authorization header exists.
        /// </summary>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }
            try
            {
                var authHeader = AuthenticationHeaderValue.Parse(Request.Headers["Authorization"]);
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["AuthSettings:Secret"]));

                var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, "user"),
                new Claim(ClaimTypes.Name, "user"),
                };

                //Generate new Token only for testing, in a real case - the token is received from the client
                string tokenAsString = GenerateToken(claims, securityKey);
                ValidateToken(tokenAsString, securityKey);

                var identity = new ClaimsIdentity(claims, Scheme.Name);
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);

                return AuthenticateResult.Success(ticket);
            }
            catch
            {
                return AuthenticateResult.Fail("Invalid Authorization Header");
            }

        }
        private string GenerateToken(Claim[] claims, SymmetricSecurityKey securityKey)
        {
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddSeconds(10),
                signingCredentials: credentials);

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }

        private async void ValidateToken(string tokenAsString, SymmetricSecurityKey securityKey) 
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            TokenValidationResult result = await tokenHandler.ValidateTokenAsync(tokenAsString, new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey
            });

        }
    }
}
