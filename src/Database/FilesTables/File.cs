using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace backendProject.Database.FilesTables
{
    public class File
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid FileId { get; set; }

        [Required]
        public Boolean IsPublic { get; set; } = false;
        public virtual ICollection<Read> ReadPermissions { get; set; }
        public virtual ICollection<Write> WritePermissions { get; set; }
    }
}