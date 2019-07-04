using System;
using System.ComponentModel.DataAnnotations;

namespace backendProject.Objects.ApiRequests
{
    public class ShareFileRequest
    {
        [Required]
        public string FileId { get; set; }

        public string PersonUniqueId { get; set; } = String.Empty;

        public Boolean ReadPermission { get; set; } = false;

        public Boolean WritePermission { get; set; } = false;

        public Boolean PublicPermission { get; set; } = false;
    }
}