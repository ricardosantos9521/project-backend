using Microsoft.IdentityModel.Tokens;

namespace Project.Backend
{
    public class JwtSettings
    {
        public static string Issuer = "rics";
        public static string Audience = "project.backend";
        public static int ExpireIn = 30;
        public static SecurityKey SecurityKey { get; set; }
    }
}