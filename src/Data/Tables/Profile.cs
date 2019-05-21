using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace backendProject.Data.Tables
{
    public class Profile
    {
        [Key, JsonIgnore, ForeignKey("Identity.UniqueId")]
        public Guid UniqueId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        public string Picture { get; set; }

        public Int64? BirthDate { get; set; }

        public String Gender { get; set; }

        [JsonIgnore]
        public virtual Identity Identity { get; set; }
    }
}