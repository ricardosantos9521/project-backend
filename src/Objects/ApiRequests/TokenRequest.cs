using System.ComponentModel.DataAnnotations;

namespace backendProject.Objects.ApiRequests
{
    public class TokenRequest
    {
        [Required]
        public string Issuer { get; set; }
        
        [Required]
        public string IdToken { get; set; }
    }
}