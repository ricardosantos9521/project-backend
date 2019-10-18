using System.ComponentModel.DataAnnotations;

namespace Project.Backend.Objects.ApiRequests
{
    public class TokenRequest
    {
        [Required]
        public string Issuer { get; set; }
        
        [Required]
        public string IdToken { get; set; }
    }
}