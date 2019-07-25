using Microsoft.IdentityModel.Tokens;

namespace backendProject
{
    public class JwtSettings
    {
        public static string Issuer = "rics";
        public static string Audience = "backendProject";
        public static int ExpireIn = 30;
        public static SecurityKey SecurityKey { get; set; }
    }
}