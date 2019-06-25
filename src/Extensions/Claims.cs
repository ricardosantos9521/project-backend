
using System.Security.Claims;

namespace backendProject.Extensions
{
    public static class Claims
    {
        public static string GetUniqueId(this ClaimsPrincipal userContext)
        {
            return userContext.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static string GetSessionId(this ClaimsPrincipal userContext)
        {
            return userContext.FindFirst("sid")?.Value;
        }
    }
}