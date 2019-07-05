using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backendProject.Database.AccountTables;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace backendProject.Database.FilesTables
{
    public class File
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid FileId { get; set; }

        [Required]
        public string FileName { get; set; }

        [Required]
        public long FileLength { get; set; }

        [Required]
        public string ContentType { get; set; }

        [Required]
        public byte[] Bytes { get; set; }

        [Required]
        public Boolean IsPublic { get; set; } = false;

        [Required]
        [JsonConverter(typeof(IsoDateTimeConverter))]
        public DateTime CreationDate { get; set; }

        [Required, ForeignKey("OwnedBy"), JsonIgnore]
        public Guid OwnedByUniqueId { get; set; }
        public virtual Profile OwnedBy { get; set; }
        public virtual ICollection<Read> ReadPermissions { get; set; }
        public virtual ICollection<Write> WritePermissions { get; set; }
    }
}