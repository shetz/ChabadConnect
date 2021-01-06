using Microsoft.AspNetCore.Mvc;
using API.Entities;
using API.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using API.interfaces;
using API.DTOs;
using AutoMapper;
using System.Security.Claims;

namespace API.Controllers
{

    public class UsersController : BaseApiController
    {
        //  private readonly DataContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _maper;
        public UsersController(IUserRepository userRepository, IMapper maper)//DataContext context)
        {
            _maper = maper;
            _userRepository = userRepository;
            //  _context = context;

        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<memberDto>>> GetUsers()
        {
            //  return await _context.Users.ToListAsync();
            //  var users = await _userRepository.GetUsersAsync();
            // var usersToReturn = _maper.Map< IEnumerable<memberDto >>(users);
            var usersToReturn = await _userRepository.GetMembersAsync();
            return Ok(usersToReturn);

        }

       
        [Authorize]
        //  [HttpGet("{id}")]
        [HttpGet("{username}")]
        public async Task<ActionResult<memberDto>> GetUser(string username)//(int id)
        {
            //in comment is tyhe evolution of this action:
            //return await _context.Users.FindAsync(id);
            //var user = await _userRepository.GetUserByUsernameAsync(username);
            // calling the mapper here works , but it takes two action == slow preformance !
            //return _maper.Map<memberDto >(user);
            var user = await _userRepository.GetMemberAsync(username);
            return user;


        }


        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto )
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var user = await _userRepository.GetUserByUsernameAsync(username);

            _maper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            if(await _userRepository.SaveAllAsync())
            return NoContent();

            return BadRequest("failed to update user");
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