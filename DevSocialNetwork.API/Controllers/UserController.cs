using DevSocialNetwork.API.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DevSocialNetwork.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public UserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("list")]
        public IActionResult GetUsers([FromQuery] int pageSize, int pageNumber)
        {
            try
            {
                var result = _userManager.Users
                        .Skip(pageSize * (pageNumber - 1))
                        .Take(pageSize)
                        .Select(x => new UserProfileResponse(x))
                        .ToList();

                return StatusCode(StatusCodes.Status200OK, new { data = result });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message });
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserProfileById(string userId)
        {
            var result = await _userManager.FindByIdAsync(userId);

            if (result is null)
                return StatusCode(StatusCodes.Status404NotFound, new Response { Message = "User not found!" });

            var userProfileResponse = new UserProfileResponse(result);

            return StatusCode(StatusCodes.Status200OK, new Response { Message = "User found!", Data = userProfileResponse });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateUserProfile(UserUpdate userUpdateRequest)
        {
            string? email = HttpContext.User?.Identity?.Name;

            if (email == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Message = "There is an error in the access token. No email claim." });

            var userToUpdate = await _userManager.FindByEmailAsync(email);

            if (userToUpdate == null)
                return StatusCode(StatusCodes.Status404NotFound, new Response { Message = "User not found!" });

            userToUpdate.FirstName = userUpdateRequest.FirstName;
            userToUpdate.LastName = userUpdateRequest.LastName;
            userToUpdate.BirthDate = userUpdateRequest.BirthDate;
            userToUpdate.Gender = userUpdateRequest.Gender;

            var result = await _userManager.UpdateAsync(userToUpdate);

            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Message = "User update failed! Please check user details and try again." });

            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}
