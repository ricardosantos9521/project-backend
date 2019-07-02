using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backendProject.Database.AdminTables;
using backendProject.Database.FilesTables;

namespace backendProject.Database.AccountTables
{
    public class Identity
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UniqueId { get; set; }

        [Required, MaxLength(50)]
        public string Issuer { get; set; }

        [Required, MaxLength(100)]
        public string SubjectId { get; set; }

        public virtual Profile Profile { get; set; }

        public virtual Admin Admin { get; set; }

        public virtual ICollection<Session> Sessions { get; set; }
        
        public virtual ICollection<Read> ReadPermissions { get; set; }
        
        public virtual ICollection<Write> WritePermissions { get; set; }
    }
}