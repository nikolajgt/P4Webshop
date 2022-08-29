using Microsoft.IdentityModel.Tokens;
using P4Webshop.Models.Base;
using P4Webshop.Models.JWT;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace P4Webshop.Extensions.JWT
{
    /// <summary>
    /// Static helper class for JWT Validation and Generatin different tokens
    /// Works with JWTManager but can easily be called by other classes
    /// </summary>
    public class JwtHelper
    {
        private static RNGCryptoServiceProvider? _cryptoService = new RNGCryptoServiceProvider();

        /// <summary>
        /// This method takes the jwt token and secret and make it to a ClaimsPrincipal (Its a way to store the users claims and use it a HttpContext.User = ClaimsPrincipal)
        /// Example can be found in CustomAuthenticationHandler.cs --> HandleAsync at the bottom.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="secret"></param>
        /// <returns></returns>
        public static ClaimsPrincipal GetClaimsPrincipal(string token, string secret)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;


                byte[] symmetricKey = Encoding.UTF8.GetBytes(secret);

                TokenValidationParameters validationParameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(symmetricKey)
                };

                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, validationParameters, out _);

                return principal;
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Here we are generating JWT token out from the paramters
        /// </summary>
        /// <param name="role"></param>
        /// <param name="guid"></param>
        /// <param name="username"></param>
        /// <param name="secret"></param>
        /// <returns></returns>

        public static string GenerateToken(Roles role, Guid guid, string username, string secret)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            byte[] jwtKey = Encoding.ASCII.GetBytes(secret);
            SecurityTokenDescriptor tokenDescripter = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Role, role.ToString()),
                    new Claim(ClaimTypes.Name, guid.ToString()),
                    new Claim(JwtRegisteredClaimNames.Sub, username)

                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(jwtKey), SecurityAlgorithms.HmacSha256Signature),
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescripter);
            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Here we are generating Refresh tokens out from the ipaddress. The reason we do so, is you are logged in and an attacker gets your JWT and refresh
        /// He cant use it, becuase it dont come from the same IP
        /// </summary>
        /// <param name="ipaddress"></param>
        /// <returns></returns>
        public static RefreshToken GenerateRefreshToken(string ipaddress)
        {
            try
            {
                using (var service = _cryptoService)
                {
                    byte[] randomBytes = new byte[64];
                    service.GetBytes(randomBytes);
                    return new RefreshToken
                    {
                        Token = Convert.ToBase64String(randomBytes),
                        Expires = DateTime.UtcNow.AddMinutes(24),
                        Created = DateTime.UtcNow,
                        CreatedByIp = ipaddress,
                    };
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
