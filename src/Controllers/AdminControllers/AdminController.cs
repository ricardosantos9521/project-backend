using System.Threading.Tasks;
using Project.Backend.Database;
using Project.Backend.Database.AdminTables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Project.Backend.Controllers.AdminControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize("IsAdmin")]
    public class AdminController : ControllerBase
    {
        private ApplicationDbContext _dbContext;
        public AdminController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult IsAdmin()
        {
            return Ok();
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddAdminAsync([FromBody] string guid)
        {
            var identity = await _dbContext.Identity.FirstOrDefaultAsync(x => x.UniqueId.ToString().Equals(guid));
            if (identity != null)
            {
                await _dbContext.Admin.AddAsync(new Admin
                {
                    UniqueId = identity.UniqueId
                });

                if (await _dbContext.SaveChangesAsync() > 0)
                {
                    return Ok();
                }
            }

            return BadRequest();
        }
    }
}
