using Microsoft.AspNetCore.Mvc;
using API.Entities;
using API.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using API.Interfaces;
using API.DTOs;
using AutoMapper;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using API.Helpers;
using API.Extensions;

namespace API.Controllers
{

    public class UsersController : BaseApiController
    {
        //  private readonly DataContext _context;
        // private readonly IUserRepository _unitOfWork.UserRepository;
        private readonly IMapper _maper;
        private readonly IPhotoService _photoService;
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IMapper maper, IPhotoService photoService, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _photoService = photoService;
            _maper = maper;
            // _unitOfWork.UserRepository = userRepository;
            //  _context = context;

        }
        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
        {
          //  var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
           var gender = await  _unitOfWork.UserRepository.GetUserGender(User.GetUsername());
           
            userParams.CurrentUsername = User.GetUsername();

            if (string.IsNullOrEmpty(userParams.Gender))
                userParams.Gender = gender == "male" ? "female" : "male";

            var usersToReturn = await _unitOfWork.UserRepository.GetMembersAsync(userParams);

            Response.AddPaginationHeader(usersToReturn.CurrentPage, usersToReturn.PageSize,
             usersToReturn.TotalCount, usersToReturn.TotalPages);

            return Ok(usersToReturn);

        }


        [Authorize]
        //  [HttpGet("{id}")]
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<ActionResult<MemberDto>> GetUser(string username)//(int id)
        {
            //in comment is tyhe evolution of this action:
            //return await _context.Users.FindAsync(id);
            //var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
            // calling the mapper here works , but it takes two action == slow preformance !
            //return _maper.Map<memberDto >(user);
            var user = await _unitOfWork.UserRepository.GetMemberAsync(username);
            return user;


        }


        [HttpPut]
        public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
        {
            var username = User.GetUsername();

            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);

            _maper.Map(memberUpdateDto, user);

            _unitOfWork.UserRepository.Update(user);

            if (await _unitOfWork.Complete())
                return NoContent();

            return BadRequest("failed to update user");
        }

        [HttpPost("{add-photo}")]
        public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

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

            if (await _unitOfWork.Complete())
            {
                // return _maper.Map<PhotoDto>(photo);

                return CreatedAtRoute("GetUser", new { username = user.UserName }, _maper.Map<PhotoDto>(photo));


            }

            return BadRequest("Problem adding photo");
        }

        [HttpPut("set-main-photo/{photoID}")]
        public async Task<ActionResult> SetMainPhoto(int photoID)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoID);

            if (photo.IsMain) return BadRequest("this is already your main photo");

            var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

            if (currentMain != null)
                currentMain.IsMain = false;

            photo.IsMain = true;

            if (await _unitOfWork.Complete())
                return NoContent();

            return BadRequest("failed to set main photo :( ");

        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoID)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoID);

            if (photo == null) return NotFound();
            if (photo.IsMain) return BadRequest("You canot delete main photo");

            if (photo.PublicId != null)
            {
                //delete from cloudinary
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("failed to delete photo :( ");
        }


        
    }
}