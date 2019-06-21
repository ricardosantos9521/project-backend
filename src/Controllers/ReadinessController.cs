using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backendProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReadinessController : ControllerBase
    {

        [AllowAnonymous]
        [HttpGet()]
        public ActionResult<Dictionary<string, string>> IsReady()
        {
            if (Startup.Readiness.Count == 3)
            {
                return Ok(Startup.Readiness);
            }
            else
            {
                return BadRequest(Startup.Readiness);
            }
        }
    }
}