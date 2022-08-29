using P4Webshop.Extensions.JWT;
using P4Webshop.Interface;
using P4Webshop.Models.Base;
using P4Webshop.Models.JWT;
using P4Webshop.Models.POCO;
using System.Security.Claims;

namespace P4Webshop.Services
{
    public class JwtService : IJwtService
    {
        private readonly SecretKeys _key;
        private readonly ILogger _logger;

        public JwtService(SecretKeys keyModel, ILoggerFactory loggerFactory)
        {
            _key = keyModel;
            _logger = loggerFactory.CreateLogger<JwtService>();
        }

        public string GetTokenFromHeader(HttpContext context)
        {
            string authHeader = context.Request.Headers["Authorization"];

            if (String.IsNullOrEmpty(authHeader))
                return null;

            string accessTokenSkippedFirst = string.Join(string.Empty, authHeader.Skip(7));

            return accessTokenSkippedFirst;
        }

        public ClaimsPrincipal TokenToClaimsPrincipal(string token)
        {
            try
            {
                var principal = JwtHelper.GetClaimsPrincipal(token, _key.JwtKey);

                if (principal.Claims == null)
                {
                    return new ClaimsPrincipal();
                }

                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{JwtManager} TokenToClaimsPrincipal error", typeof(JwtService));
                return new ClaimsPrincipal();
            }
        }


        public string GenerateJwtToken(Roles role, Guid guid, string username)
        {
            try
            {
                var token = JwtHelper.GenerateToken(role, guid, username, _key.JwtKey);
                if (token == null)
                {
                    return "";
                }

                return token;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{JwtManager} GenerateJwtToken error", typeof(JwtService));
                return "";
            }
        }

        public RefreshToken GenerateRefreshToken(string ipaddress)
        {
            try
            {
                var refreshToken = JwtHelper.GenerateRefreshToken(ipaddress);

                if (refreshToken == null)
                {
                    return new RefreshToken();
                }

                return refreshToken;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "{JwtManager} GenerateRefreshToken error", typeof(JwtService));
                return new RefreshToken();
            }
        }

        public bool IsHealthy()
        {
            return true;
        }
    }
}
