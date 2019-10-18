using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Project.Backend.Database.AccountTables
{
    public class Session
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SessionId { get; set; }

        [Required, JsonIgnore]
        public Guid UniqueId { get; set; }
        public DateTime FirstLogin { get; set; }
        public DateTime LastLogin { get; set; }

        [JsonIgnore]
        public virtual Identity Identity { get; set; }

        [JsonIgnore]
        public virtual RefreshToken RefreshToken { get; set; }
    }
}
