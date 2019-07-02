using System;
using System.Linq;
using backendProject.Database.FilesTables;

namespace backendProject.Objects.ApiResponses
{
    public class FileDescription
    {
        public FileDescription() { }
        public FileDescription(FileBytes file, Guid uniqueId)
        {
            FileId = file.FileId;
            ContentType = file.ContentType;
            FileLength = file.FileLength;
            FileName = file.FileName;
            IsPublic = file.IsPublic;
            ReadPermission = file.ReadPermissions.Any(y => y.UniqueId == uniqueId);
            WritePermission = file.WritePermissions.Any(y => y.UniqueId == uniqueId);
            CreationDate = file.CreationDate;
        }

        public Guid FileId { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public long FileLength { get; set; }
        public Boolean IsPublic { get; set; }
        public Boolean WritePermission { get; set; }
        public Boolean ReadPermission { get; set; }
        public DateTime CreationDate { get; set; }
    }
}