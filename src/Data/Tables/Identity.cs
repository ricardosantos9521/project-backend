using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backendProject.Data.Tables
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