using System.Diagnostics.CodeAnalysis;

namespace GarageCoreMVC.Middleware
{
    [ExcludeFromCodeCoverage]
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public TokenMiddleware(RequestDelegate next, ILoggerFactory logFcr)
        {
            _next = next;
            _logger = logFcr.CreateLogger<TokenMiddleware>();
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var receivedToken = httpContext.Request.Cookies["JWT-Credential"];
            if(receivedToken != null)
                httpContext.Request.Headers.Append("Authorization", "Bearer " + receivedToken);
            await _next(httpContext);
        }
    }

    [ExcludeFromCodeCoverage]
    public static class TokenMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenMiddleware>();
        }
    }
}

