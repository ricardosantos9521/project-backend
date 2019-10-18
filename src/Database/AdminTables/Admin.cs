using System;
using System.ComponentModel.DataAnnotations;
using Project.Backend.Database.AccountTables;
using Newtonsoft.Json;

namespace Project.Backend.Database.AdminTables
{
    public class Admin
    {
        [Key, JsonIgnore]
        public Guid UniqueId { get; set; }

        [JsonIgnore]
        public virtual Identity Identity { get; set; }
    }
}