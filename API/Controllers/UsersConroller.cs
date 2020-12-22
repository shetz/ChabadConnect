using Microsoft.AspNetCore.Mvc;
using API.Entities;
using API.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly DataContext _context;
        public UsersController(DataContext context)
        {
            _context = context;

        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return await  _context.Users.ToListAsync();


        }

        //api/users/3
        [HttpGet("{id}")]
        public async Task< ActionResult<AppUser>> GetUser(int id)
        {
            return await _context.Users.FindAsync(id);


        }

        // [HttpGet]
        // public ActionResult<IEnumerable<AppUser>> GetUsers()
        // {
        //     return _context.Users.ToList();

           
        // }

        //     //api/users/3
        // [HttpGet("{id}")]
        // public ActionResult<AppUser> GetUser(int id)
        // {
        //     return _context.Users.Find(id);

          
        // }
    }
}