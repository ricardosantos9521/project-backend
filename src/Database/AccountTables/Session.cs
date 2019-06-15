using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace backendProject.Database.AccountTables
{
    public class Session
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SessionId { get; set; }

        [Required]
        public Guid UniqueId { get; set; }

        [JsonIgnore]
        public virtual Identity Identity { get; set; }

        [JsonIgnore]
        public virtual RefreshToken RefreshToken { get; set; }
    }
}
