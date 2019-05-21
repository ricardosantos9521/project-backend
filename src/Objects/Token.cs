using System;

namespace backendProject.Objects
{
    public class TokenObject
    {
        public string Token { get; set; }

        public DateTime ExpireUtc { get; set; }
    }
}