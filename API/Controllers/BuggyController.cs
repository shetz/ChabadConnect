using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class BuggyController : BaseApiController
    {
        private readonly DataContext _context;
        public BuggyController(DataContext context)
        {
            _context = context;

        }

        [HttpGet("auth")]
        public ActionResult<string> GetSecret()
        {
            return Unauthorized();
        }

        [HttpGet("not-found")]
        public ActionResult<AppUser> GetNotFound()
        {
            var thing = _context.Users.Find(-1);
            
            if(thing == null) return NotFound();

            return Ok(thing);
        }

        [HttpGet("Server-error")]
        public ActionResult<string> GetServerEror()
        {
            var thing = _context.Users.Find(-1);
            var thingTorReturn = thing.ToString ();
            
            return thingTorReturn;
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest()
        {
            return BadRequest("this was not a good request");
        }
    }
}