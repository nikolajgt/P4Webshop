using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using P4Webshop.Interface;
using P4Webshop.Models.Base;
using P4Webshop.Models.HTTP;
using System.Net;

namespace P4Webshop.Extensions.JWT
{
    /// <summary>
    ///  Middleware for sending different response models to user depending on if they get authorized, have the correct permissions or not.
    ///  In short this works like a firewall, if it dont have the requirements/Permissions the http request is sent back with a ErrorResponseModel
    ///  
    ///  This class works like a Singleton instance, so we cant just use AddScoped Services so the way we do it is accessing the
    ///  IServiceScopeFactory and use that to initilize our AddScoped services as an singleton instance.
    /// </summary>
    /// <param name="next"></param>
    /// <param name="context"></param>
    /// <param name="policy"></param>
    /// <param name="authorizeResult"></param>
    /// <returns></returns>

    public class CustomAuthenticationHandler : IAuthorizationMiddlewareResultHandler
    {
        private readonly AuthorizationMiddlewareResultHandler DefaultHandler = new AuthorizationMiddlewareResultHandler();
        private readonly IServiceScopeFactory _scopeFactory;

        public CustomAuthenticationHandler(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        /// <summary>
        /// Gets executed automatic and checks if everything is correct, if not dont allow conneciton and return error
        /// </summary>
        /// <param name="next"></param>
        /// <param name="context"></param>
        /// <param name="policy"></param>
        /// <param name="authorizeResult"></param>
        /// <returns></returns>
        public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var jwtManager = scope.ServiceProvider.GetService<IJwtService>();

                string token = jwtManager.GetTokenFromHeader(context);

                /// If token is invalid (Expired, fake token etc...)
                if (authorizeResult.Challenged || String.IsNullOrEmpty(token))
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await context.Response.WriteAsJsonAsync(new ErrorResponseModel(ResponseCode.UnAuthorize, "Unauthorized: Access is denied"));
                    return;
                }

                ///if token dont have the correct role permission (Roles = fx Admin etc..) 
                if (authorizeResult.Forbidden)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    await context.Response.WriteAsJsonAsync(new ErrorResponseModel(ResponseCode.UnAuthorize, "Permission: You dont have the required permissions"));
                    return;
                }

                var claims = jwtManager.TokenToClaimsPrincipal(token);

                ///if token is valid and have all permisison, send the http request to controller and set httpContext.User
                ///so we can call httpContext.User on customer Controller and gets the claims from the loggedin User
                if (authorizeResult.Succeeded)
                {
                    
                    if (token == null)
                    {
                        await context.Response.WriteAsJsonAsync(new ErrorResponseModel(ResponseCode.Error, "JWT Error: No token in auth header"));
                    }

                    context.User = claims;
                    await DefaultHandler.HandleAsync(next, context, policy, authorizeResult);
                }
            }
        }
     
    }
}
