using System;
using backendProject.Database.AccountTables;

namespace backendProject.Database.FilesTables
{
    public class Write
    {
        public Guid FileId { get; set; }

        public virtual File File { get; set; }

        public Guid UniqueId { get; set; }

        public virtual Identity Identity { get; set; }
    }
}