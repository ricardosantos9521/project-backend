using System;

namespace backendProject.Objects.ApiResponses
{
    public class TokensResponse
    {
        public TokenObject AccessToken { get; set; }

        public TokenObject RefreshToken { get; set; }

        public Boolean isAccountNew { get; set; }
    }
}