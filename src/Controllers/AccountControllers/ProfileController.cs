using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using backendProject.Data;
using backendProject.Data.Tables;
using backendProject.Extensions;
using backendProject.Objects.ApiRequests;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace backendProject.Controllers.AccountControllers
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
            var uniqueid = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var profile = await _dbContext.Profile.Include(x => x.Identity.Admin).FirstOrDefaultAsync(x => x.UniqueId.ToString() == uniqueid);
            if (profile != null)
            {
                return Ok(profile);
            }

            return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult> ChangeProfile([FromBody] RequestChangeProfile requestChange)
        {
            var uniqueid = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            //prevent errors
            requestChange.profile.UniqueId = new Guid(uniqueid);

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
