using System;
using System.ComponentModel.DataAnnotations;
using backendProject.Database.AccountTables;
using Newtonsoft.Json;

namespace backendProject.Database.AdminTables
{
    public class Admin
    {
        [Key, JsonIgnore]
        public Guid UniqueId { get; set; }

        [JsonIgnore]
        public virtual Identity Identity { get; set; }
    }
}