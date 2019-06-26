using System;
using System.IO;
using System.Threading.Tasks;
using backendProject.Database;
using backendProject.Extensions;
using backendProject.Database.FilesTables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace backendProject.Controllers.FileController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FileController : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        public FileController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<byte[]> UploadFileAsBlob(Stream stream, string filename, long length)
        {
            var bytes = new byte[length];
            await stream.ReadAsync(bytes, 0, 1000000);
            return bytes;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> PostProfilePicture(IFormFile file)
        {
            var uniqueId = User.GetUniqueId();

            var stream = file.OpenReadStream();

            var bytes = await UploadFileAsBlob(stream, file.FileName, file.Length);

            var fileTable = new FileBytes
            {
                Bytes = bytes,
                ContentType = file.ContentType,
                FileName = file.FileName,
                FileLength = file.Length,
                IsPublic = false,
                WritePermissions = new List<Write>()
                {
                    new Write
                    {
                        UniqueId = new Guid(uniqueId)
                    }
                },
                ReadPermissions = new List<Read>()
                {
                    new Read
                    {
                        UniqueId = new Guid(uniqueId)
                    }
                }
            };

            await _dbContext.File.AddAsync(fileTable);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
