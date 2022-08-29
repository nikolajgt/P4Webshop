
using P4Webshop.Models.Base;
using P4Webshop.Models.JWT;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;


namespace P4Webshop.Interface
{
    public interface IJwtService
    {
        string GetTokenFromHeader(HttpContext context);
        ClaimsPrincipal TokenToClaimsPrincipal(string token);
        string GenerateJwtToken(Roles role, Guid userid, string username);
        RefreshToken GenerateRefreshToken(string ipaddress);
        bool IsHealthy();
    }
}
