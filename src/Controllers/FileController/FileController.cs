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
using System.Linq;
using backendProject.Objects.ApiResponses;

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

        public async Task<byte[]> UploadFileAsBlob(Stream stream, long totalLength)
        {
            var bytes = new byte[totalLength];
            long position = 0;
            int bytesToRead = (totalLength <= 1000000) ? Convert.ToInt32(totalLength) : 1000000;

            while (bytesToRead != 0)
            {
                var bytesAux = new byte[bytesToRead];
                await stream.ReadAsync(bytesAux, 0, bytesToRead);
                bytesAux.CopyTo(bytes, position);

                totalLength -= bytesToRead;
                position += bytesToRead;
                bytesToRead = (totalLength <= 1000000) ? Convert.ToInt32(totalLength) : 1000000;
            }

            return bytes;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> PostProfilePicture(IFormFile file)
        {
            var uniqueId = User.GetUniqueId();

            var stream = file.OpenReadStream();

            var bytes = await UploadFileAsBlob(stream, file.Length);

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
                },
                CreationDate = DateTime.UtcNow
            };

            await _dbContext.File.AddAsync(fileTable);

            if (await _dbContext.SaveChangesAsync() > 0)
            {
                return Ok(new FileDescription(fileTable, new Guid(uniqueId)));
            };

            return BadRequest();

        }

        [HttpGet("info/{fileId}")]
        public async Task<IActionResult> GetFileInfo(string fileId)
        {
            var uniqueId = User.GetUniqueId();

            var file = await _dbContext.File.Include(x => x.ReadPermissions).Include(x => x.WritePermissions)
                                .Where(x =>
                                    x.FileId == new Guid(fileId) && (
                                        x.ReadPermissions.Any(y => y.UniqueId == new Guid(uniqueId)) ||
                                        x.WritePermissions.Any(y => y.UniqueId == new Guid(uniqueId)) ||
                                        x.IsPublic
                                    )
                                )
                                .OrderByDescending(x => x.CreationDate)
                                .Select(x =>
                                    new FileDescription(x, new Guid(uniqueId))
                                )
                                .FirstOrDefaultAsync();

            if (file != null)
            {
                return Ok(file);
            }

            return BadRequest("File doesn't exist or you don't have permissions to it!");
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

        [HttpGet("files")]
        public async Task<IActionResult> GetFilesInfo()
        {
            var uniqueId = User.GetUniqueId();

            var list = await _dbContext.File.Include(x => x.ReadPermissions).Include(x => x.WritePermissions)
                                .Where(x =>
                                   x.ReadPermissions.Any(y => y.UniqueId == new Guid(uniqueId)) ||
                                   x.WritePermissions.Any(y => y.UniqueId == new Guid(uniqueId))
                                )
                                .OrderByDescending(x => x.CreationDate)
                                .Select(x =>
                                    new FileDescription(x, new Guid(uniqueId))
                                )
                                .ToListAsync();

            if (list != null)
            {
                return Ok(list);
            }

            return BadRequest("Something happen try again later!");
        }
    }
}