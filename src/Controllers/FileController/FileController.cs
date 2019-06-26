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
using Microsoft.EntityFrameworkCore;

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

            if (await _dbContext.SaveChangesAsync() > 0)
            {
                return Ok(fileTable.FileId);
            };

            return BadRequest();

        }

        [HttpGet("get/{fileId}")]
        public async Task<IActionResult> GetFile(string fileId)
        {
            var uniqueId = User.GetUniqueId();

            var file = await _dbContext.ReadPermissions.Include(x => x.File).FirstOrDefaultAsync(x => x.FileId == new Guid(fileId) && x.UniqueId == new Guid(uniqueId));

            if (file != null)
            {
                return File(file.File.Bytes, file.File.ContentType, file.File.FileName);
            }

            return BadRequest("File doesn't exist or you don't have permissions to it!");
        }
    }
}
