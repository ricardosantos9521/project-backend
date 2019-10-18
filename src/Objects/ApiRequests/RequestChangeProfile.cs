using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Project.Backend.Database.AccountTables;

namespace Project.Backend.Objects.ApiRequests
{
    public class RequestChangeProfile
    {
        [Required]
        public Profile profile { get; set; }

        [Required]
        public List<string> propertieschanged { get; set; }
    }
}