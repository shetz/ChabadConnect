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
using Microsoft.AspNetCore.Http;
using API.Extentions;

namespace API.Controllers
{

    public class UsersController : BaseApiController
    {
        //  private readonly DataContext _context;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _maper;
        private readonly IPhotoService _photoService;

        public UsersController(IUserRepository userRepository,
        IMapper maper, IPhotoService photoService)
        {
            _photoService = photoService;
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
        [HttpGet("{username}", Name = "GetUser")]
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
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.GetUserName();

            var user = await _userRepository.GetUserByUsernameAsync(username);

            _maper.Map(memberUpdateDto, user);

            _userRepository.Update(user);

            if (await _userRepository.SaveAllAsync())
                return NoContent();

            return BadRequest("failed to update user");
        }

        [HttpPost("{add-photo}")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo()
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId


            };

            if (user.Photos.Count == 0)
                photo.IsMain = true;

            user.Photos.Add(photo);

            if (await _userRepository.SaveAllAsync())
            {
                // return _maper.Map<PhotoDto>(photo);

                return CreatedAtRoute("GetUser", new { username = user.UserName }, _maper.Map<PhotoDto>(photo));


            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoID}")]
        public async Task<ActionResult> SetMainPhoto(int photoID)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoID);

            if (photo.IsMain) return BadRequest("this is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

            if (currentMain != null)
                currentMain.IsMain = false;

            photo.IsMain = true;

            if (await _userRepository.SaveAllAsync())
                return NoContent();

            return BadRequest("failed to set main photo :( ");

        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoID)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUserName());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoID);

            if (photo== null) return NotFound();
            if(photo.IsMain) return BadRequest("You canot delete main photo");

            if(photo.PublicId !=null)
            {
                //delete from cloudinary
               var result = await _photoService.DeletePhotoAsync(photo.PublicId);
               if(result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);
          
            if (await _userRepository.SaveAllAsync()) return Ok();

            return BadRequest("failed to delete photo :( ");
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