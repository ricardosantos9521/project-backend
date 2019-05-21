using System.ComponentModel.DataAnnotations;

namespace backendProject.Objects.ApiRequests
{
    public class TokenRefreshRequest
    {
        [Required]
        public string Token { get; set; }
    }
}