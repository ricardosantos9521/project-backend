using System;
using System.Linq;
using Project.Backend.Database.FilesTables;

namespace Project.Backend.Objects.ApiResponses
{
    public class FileDescription
    {
        public FileDescription() { }
        public FileDescription(File file, Guid uniqueId)
        {
            FileId = file.FileId;
            ContentType = file.ContentType;
            FileLength = file.FileLength;
            FileName = file.FileName;
            IsPublic = file.IsPublic;
            ReadPermission = file.ReadPermissions.Any(y => y.UniqueId == uniqueId) || file.OwnedByUniqueId == uniqueId;
            WritePermission = file.WritePermissions.Any(y => y.UniqueId == uniqueId) || file.OwnedByUniqueId == uniqueId;
            CreationDate = file.CreationDate;
            CreatedBy = new CreatedByProfile
            {
                FirstName = file.OwnedBy.FirstName,
                LastName = file.OwnedBy.LastName,
                UniqueId = file.OwnedBy.UniqueId
            };
            ReadPermissionCount = file.ReadPermissions.Distinct().Count();
            WritePermissionCount = file.WritePermissions.Distinct().Count();
        }

        public Guid FileId { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public long FileLength { get; set; }
        public Boolean IsPublic { get; set; }
        public Boolean WritePermission { get; set; }
        public Boolean ReadPermission { get; set; }
        public DateTime CreationDate { get; set; }
        public CreatedByProfile CreatedBy { get; set; }
        public Int64 WritePermissionCount { get; set; }
        public Int64 ReadPermissionCount { get; set; }
    }

    public class CreatedByProfile
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid UniqueId { get; set; }
    }
}