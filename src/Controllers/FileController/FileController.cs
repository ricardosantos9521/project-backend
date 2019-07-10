using System;
using System.IO;
using System.Threading.Tasks;
using backendProject.Database;
using backendProject.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using backendProject.Objects.ApiResponses;
using backendProject.Objects.ApiRequests;
using backendProject.Database.FilesTables;

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
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var uniqueId = User.GetUniqueId();

            var stream = file.OpenReadStream();

            var bytes = await UploadFileAsBlob(stream, file.Length);

            var fileTable = new Database.FilesTables.File
            {
                Bytes = bytes,
                ContentType = file.ContentType,
                FileName = file.FileName,
                FileLength = file.Length,
                IsPublic = false,
                CreationDate = DateTime.UtcNow,
                OwnedByUniqueId = new Guid(uniqueId)
            };

            await _dbContext.File.AddAsync(fileTable);

            if (await _dbContext.SaveChangesAsync() > 0)
            {
                var fileAux = await _dbContext.File.Include(x => x.ReadPermissions).Include(x => x.WritePermissions).Include(x => x.OwnedBy)
                                            .FirstOrDefaultAsync(x => x.OwnedByUniqueId == fileTable.OwnedByUniqueId);

                if (fileAux != null)
                {
                    return Ok(new FileDescription(fileAux, new Guid(uniqueId)));
                }
            };

            return BadRequest();
        }

        [HttpGet("info/{fileId}")]
        public async Task<IActionResult> GetFileInfo(string fileId)
        {
            var uniqueId = User.GetUniqueId();

            var file = await _dbContext.File.Include(x => x.ReadPermissions).Include(x => x.WritePermissions).Include(x => x.OwnedBy)
                                .Where(x =>
                                    x.FileId == new Guid(fileId) && (
                                        x.OwnedByUniqueId == new Guid(uniqueId) ||
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

            var file = await _dbContext.File.Include(x => x.ReadPermissions)
                .FirstOrDefaultAsync(x =>
                    x.FileId == new Guid(fileId) && (
                        x.OwnedByUniqueId == new Guid(uniqueId) ||
                        x.ReadPermissions.Any(y => y.UniqueId == new Guid(uniqueId)) ||
                        x.IsPublic
                    )
                );

            if (file != null)
            {
                return File(file.Bytes, file.ContentType, file.FileName);
            }

            return BadRequest("File doesn't exist or you don't have permissions to it!");
        }

        [HttpGet("files")]
        public async Task<IActionResult> GetFilesInfo()
        {
            var uniqueId = User.GetUniqueId();

            var list = await _dbContext.File.Include(x => x.ReadPermissions).Include(x => x.WritePermissions).Include(x => x.OwnedBy)
                                .Where(x =>
                                   x.OwnedByUniqueId == new Guid(uniqueId) ||
                                   x.ReadPermissions.Any(y => y.UniqueId == new Guid(uniqueId)) ||
                                   x.WritePermissions.Any(y => y.UniqueId == new Guid(uniqueId)) ||
                                   x.IsPublic
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

        [HttpPost("share")]
        public async Task<IActionResult> ShareFile([FromBody]ShareFileRequest shareRequest)
        {
            var uniqueId = User.GetUniqueId();

            var file = await _dbContext.File.Include(x => x.ReadPermissions).Include(x => x.WritePermissions)
                                .Where(x =>
                                    x.FileId == new Guid(shareRequest.FileId) &&
                                    (
                                       x.OwnedByUniqueId == new Guid(uniqueId) ||
                                       x.WritePermissions.Any(y => y.UniqueId == new Guid(uniqueId))
                                    )
                                )
                                .FirstOrDefaultAsync();

            if (file != null)
            {
                _dbContext.File.Attach(file);

                if (!shareRequest.Email.Equals(String.Empty))
                {
                    var user = await _dbContext.Profile.FirstOrDefaultAsync(x => x.Email.Equals(shareRequest.Email));
                    if (user != null)
                    {
                        if (shareRequest.WritePermission && !file.WritePermissions.Any(y => y.UniqueId == user.UniqueId))
                        {
                            file.WritePermissions.Add(new Write
                            {
                                SharedByUniqueId = new Guid(uniqueId),
                                UniqueId = user.UniqueId,
                            });

                            file.ReadPermissions.Add(new Read
                            {
                                SharedByUniqueId = new Guid(uniqueId),
                                UniqueId = user.UniqueId,
                            });
                        }
                        else if (shareRequest.ReadPermission && !file.ReadPermissions.Any(y => y.UniqueId == user.UniqueId))
                        {
                            file.ReadPermissions.Add(new Read
                            {
                                SharedByUniqueId = new Guid(uniqueId),
                                UniqueId = user.UniqueId,
                            });
                        }
                    }
                    else
                    {
                        var NotAcceptable = BadRequest($"User with email '{shareRequest.Email}' don't exist!");
                        NotAcceptable.StatusCode = 406;

                        return NotAcceptable;
                    }
                }
                else if (shareRequest.Email.Equals(String.Empty) && (shareRequest.WritePermission || shareRequest.ReadPermission))
                {
                    var NotAcceptable = BadRequest("Email required!");
                    NotAcceptable.StatusCode = 406;

                    return NotAcceptable;
                }


                if (shareRequest.PublicPermission)
                {
                    file.IsPublic = true;
                }


                if (await _dbContext.SaveChangesAsync() >= 0)
                {
                    return Ok();
                }
            }

            return BadRequest("Something happen try again later!");
        }
    }
}
