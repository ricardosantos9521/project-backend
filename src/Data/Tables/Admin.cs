using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace backendProject.Data.Tables
{
    public class Admin
    {
        [Key, JsonIgnore]
        public Guid UniqueId { get; set; }

        [JsonIgnore]
        public virtual Identity Identity { get; set; }
    }
}