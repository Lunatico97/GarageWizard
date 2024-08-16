using System.Diagnostics.CodeAnalysis;

namespace GarageCoreMVC.Common.Configurations
{
    public static class GlobalConfig
    {
        public const string ConnectionStringPSQL = "PSQLVdb";
        public const string JWTSecretKey = "@m@z!ng!--0__0--!gn!z@m@@m@z!ng!--0__0--!gn!z@m@@m@z!ng!--0__0--!gn!z@m@@m@z!ng!--0__0--!gn!z@m@";
        public const string JWTIssuer = "http://localhost:5062";
        public const string JWTAudience = "http://localhost:5062";
        public const double JWTExpiryHours = 2;
        public const string GeneratedTokenCookieName = "JWT-Credential";
    }
}
