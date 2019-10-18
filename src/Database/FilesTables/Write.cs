using System;
using System.ComponentModel.DataAnnotations.Schema;
using Project.Backend.Database.AccountTables;
using Newtonsoft.Json;

namespace Project.Backend.Database.FilesTables
{
    public class Write
    {
        [ForeignKey("SharedByIdentity")]
        public Guid SharedByUniqueId { get; set; }

        public virtual Identity SharedByIdentity { get; set; }

        public Guid FileId { get; set; }

        public virtual File File { get; set; }

        [JsonIgnore]
        public Guid UniqueId { get; set; }

        public virtual Identity Identity { get; set; }
    }
}