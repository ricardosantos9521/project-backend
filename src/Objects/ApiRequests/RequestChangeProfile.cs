using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using backendProject.Data.Tables;

namespace backendProject.Objects.ApiRequests
{
    public class RequestChangeProfile
    {
        [Required]
        public Profile profile { get; set; }

        [Required]
        public List<string> propertieschanged { get; set; }
    }
}