using GarageCoreMVC.Common;
using GarageCoreMVC.Common.Utilities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;

namespace GarageCoreMVC
{
    [ExcludeFromCodeCoverage]
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public AuthenticationMiddleware(RequestDelegate next, ILoggerFactory logFcr)
        {
            _next = next;
            _logger = logFcr.CreateLogger<AuthenticationMiddleware>();
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            _logger.LogInformation("Authentication Middleware executing ..................");
            //_logger.LogInformation("Return URL: ", httpContext.Request.Query["ReturnUrl"]);
            if (!httpContext.Request.Headers.ContainsKey("Authorization"))
            {
                if (httpContext.Request.Path.Value != "/Account/Login")
                {
                    if (httpContext.Request.Path.Value == "/Account/Register")
                    {
                        await _next(httpContext);
                        return;
                    }
                    else
                    {
                        httpContext.Response.StatusCode = (int)HttpStatusCode.Redirect;
                        httpContext.Response.Redirect("/Account/Login");
                        return;
                    } 
                }
            }
            else
            {

                var token = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                string? name = JWTHandler.ValidateSecurityToken(token, JwtRegisteredClaimNames.UniqueName);
                _logger.LogInformation($"Decrypted name: {name}");
                if(name == null)
                {
                    httpContext.Response.StatusCode= (int)HttpStatusCode.Unauthorized;
                    await httpContext.Response.WriteAsync(DefaultValues.AuthMWTokenExpiryMessage);
                    return;
                }
                else if(name != httpContext.Request.Cookies["Unique"])
                {
                    httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await httpContext.Response.WriteAsync(DefaultValues.AuthMWBadTokenMessage);
                    return;
                }
                else
                {
                    //_logger.LogInformation("Actual authenticated name: ", httpContext.User.Identity.Name);
                    _logger.LogInformation("Authenticated name: {0}", name);
                }       
            }
            await _next(httpContext);
        }
    }

    [ExcludeFromCodeCoverage]
    public static class AuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuthenticationMiddleware>();
        }
    }
}
