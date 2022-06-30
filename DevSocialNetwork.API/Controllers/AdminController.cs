using DevSocialNetwork.API.Helpers;
using DevSocialNetwork.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DevSocialNetwork.API.Controllers
{
    [Authorize(Roles = UserRoles.Admin)]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminController(
            UserManager<ApplicationUser> userManager
        )
        {
            _userManager = userManager;
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var email = HttpContext.User?.Identity?.Name;

            if (email == null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Message = "There is an error in the access token. No email claim." });

            var userToDelete = await _userManager.FindByIdAsync(userId);

            if (userToDelete.Email == email)
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Message = "Admin cannot delete itself!" });

            var result = await _userManager.DeleteAsync(userToDelete);

            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Message = "User deletion failed! Please check user details and try again." });

            return StatusCode(StatusCodes.Status204NoContent);
        }
    }
}
