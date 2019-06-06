using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using backendProject.Database.AdminTables;

namespace backendProject.Database.AccountTables
{
    public class Identity
    {
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid UniqueId { get; set; }

        [Required]
        public string Issuer { get; set; }

        [Required]
        public string SubjectId { get; set; }

        public virtual Profile Profile { get; set; }

        public virtual Admin Admin { get; set; }
    }
}