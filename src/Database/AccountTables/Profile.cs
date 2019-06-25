using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace backendProject.Database.AccountTables
{
    public class Profile
    {
        [Key, JsonIgnore]
        public Guid UniqueId { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        public string Picture { get; set; }

        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime? BirthDate { get; set; }

        public String Gender { get; set; }

        [NotMapped]
        public Boolean IsAdmin
        {
            get
            {
                if (Identity != null && Identity.Admin != null)
                {
                    return true;
                }
                return false;
            }
        }

        [JsonIgnore]
        public virtual Identity Identity { get; set; }
    }
}