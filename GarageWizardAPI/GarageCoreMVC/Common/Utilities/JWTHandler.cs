using GarageCoreMVC.Common.Configurations;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace GarageCoreMVC.Common.Utilities
{
    [ExcludeFromCodeCoverage]
    public static class JWTHandler
    {
        public static string GenerateSecurityToken(ClaimsIdentity claims, double expireHours)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GlobalConfig.JWTSecretKey));
            var handler = new JwtSecurityTokenHandler();
            var descriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.Now.AddHours(expireHours),
                Audience = GlobalConfig.JWTAudience,
                Issuer = GlobalConfig.JWTIssuer,
                SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
            };
            SecurityToken token = handler.CreateToken(descriptor);
            return handler.WriteToken(token);
        }

        public static string? ValidateSecurityToken(string token, string getKey)
        {
            if (string.IsNullOrEmpty(token))
            {
                return null;
            }
            var handler = new JwtSecurityTokenHandler();
            try
            {
                handler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = GlobalConfig.JWTIssuer,
                    ValidAudience = GlobalConfig.JWTAudience,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero, // Instant expiry after time
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GlobalConfig.JWTSecretKey))
                }, out SecurityToken validatedToken);
                var JWTToken = (JwtSecurityToken) validatedToken;
                return JWTToken.Claims.FirstOrDefault(c => c.Type == getKey)?.Value ;
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
                return null;
            }
        }
    }
}

//// Testing JWT -------------------------------------------------------------
//var claims = new ClaimsIdentity();
//claims.AddClaim(new Claim("Username", user.UserName));
//var token = JWTHandler.GenerateSecurityToken(claims, 1);
//Console.WriteLine(token.ToString());
//Console.WriteLine(JWTHandler.ValidateSecurityToken(token, "Username"));
//// Testing JWT -------------------------------------------------------------