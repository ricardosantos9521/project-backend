using System;
using System.ComponentModel.DataAnnotations;

namespace backendProject.Objects.ApiRequests
{
    public class ShareFileRequest
    {
        [Required]
        public string FileId { get; set; }

        [Required]
        public string PersonUniqueId { get; set; }

        [Required]
        public Boolean ReadPermission { get; set; } = false;

        [Required]
        public Boolean WritePermission { get; set; } = false;

        [Required]
        public Boolean PublicPermission { get; set; } = false;
    }
}