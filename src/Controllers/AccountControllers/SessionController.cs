

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using backendProject.Database;
using backendProject.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backendProject.Controllers.AccountControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SessionController : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        public SessionController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("sessions")]
        public async Task<ActionResult> GetSessions()
        {
            var uniqueId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

            var profile = await _dbContext.Identity.Include(x => x.Sessions).FirstOrDefaultAsync(x => x.UniqueId.ToString() == uniqueId);
            if (profile != null)
            {
                return Ok(profile.Sessions);
            }

            return BadRequest();
        }

        [HttpPost("delete")]
        public async Task<ActionResult> DeleteSession([FromBody] string sessionIdToDelete)
        {
            var uniqueId = User.GetUniqueId();

            var sessionId = User.GetSessionId();

            if (sessionId.Equals(sessionIdToDelete))
            {
                return BadRequest("Cannot delete your own session!");
            }

            var sessionIdToDeleteGuid = new Guid(sessionIdToDelete);

            var session = await _dbContext.Session.FirstOrDefaultAsync(x => x.UniqueId.ToString() == uniqueId && x.SessionId == sessionIdToDeleteGuid);
            if (session != null)
            {
                _dbContext.Session.Remove(session);
                if (await _dbContext.SaveChangesAsync() > 0)
                {
                    return Ok();
                }
            }
            else
            {
                return BadRequest("You don't own the sessionId or the session don't exist already!");
            }

            return BadRequest("Something happen");
        }

        [HttpPost("logout")]
        public async Task<ActionResult> LogoutSession()
        {
            var uniqueId = User.GetUniqueId();

            var sessionId = User.GetSessionId();

            var sessionIdToDeleteGuid = new Guid(sessionId);

            var session = await _dbContext.Session.FirstOrDefaultAsync(x => x.UniqueId.ToString() == uniqueId && x.SessionId == sessionIdToDeleteGuid);
            if (session != null)
            {
                _dbContext.Session.Remove(session);
                if (await _dbContext.SaveChangesAsync() > 0)
                {
                    return Ok();
                }
            }

            return BadRequest("Something happen");
        }
    }
}