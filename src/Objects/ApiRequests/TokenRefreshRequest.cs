using System.ComponentModel.DataAnnotations;

namespace Project.Backend.Objects.ApiRequests
{
    public class TokenRefreshRequest
    {
        [Required]
        public string Token { get; set; }
    }
}