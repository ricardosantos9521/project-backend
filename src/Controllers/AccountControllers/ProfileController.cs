using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Project.Backend.Database;
using Project.Backend.Extensions;
using Project.Backend.Objects.ApiRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Project.Backend.Controllers.AccountControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        public ProfileController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult> GetProfile()
        {
            var uniqueId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var profile = await _dbContext.Profile.Include(x => x.Identity.Admin).FirstOrDefaultAsync(x => x.UniqueId.ToString() == uniqueId);
            if (profile != null)
            {
                return Ok(profile);
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult> ChangeProfile([FromBody] RequestChangeProfile requestChange)
        {
            var uniqueId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //prevent errors
            requestChange.profile.UniqueId = new Guid(uniqueId);

            _dbContext.UpdateOnlyChangedProperties(requestChange.profile, requestChange.propertieschanged);

            var changes = await _dbContext.SaveChangesAsync();

            if (changes > 0)
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}
