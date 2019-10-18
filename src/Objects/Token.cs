using System;

namespace Project.Backend.Objects
{
    public class TokenObject
    {
        public string Token { get; set; }

        public DateTime ExpireUtc { get; set; }
    }
}