using DevSocialNetwork.API.Helpers;
using DevSocialNetwork.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace DevSocialNetwork.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
        {
            var user = await _userManager.FindByEmailAsync(userLogin.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, userLogin.Password))
            {
                var userRoles = await _userManager.GetRolesAsync(user);

                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                };

                foreach (var userRole in userRoles)
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));

                var token = GetToken(authClaims);

                return StatusCode(
                    StatusCodes.Status200OK,
                    new
                    {
                        token = new JwtSecurityTokenHandler().WriteToken(token),
                        expiration = token.ValidTo
                    }
                );
            }

            return Unauthorized();
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistration userRegistration)
        {
            var userExists = await _userManager.FindByEmailAsync(userRegistration.Email);

            if (userExists != null)
                return StatusCode(StatusCodes.Status400BadRequest, new { message = $"User already register with email {userRegistration.Email}" });

            ApplicationUser user = new()
            {
                Email = userRegistration.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = userRegistration.Email,
                FirstName = userRegistration.FirstName,
                LastName = userRegistration.LastName,
                BirthDate = userRegistration.BirthDate,
                Gender = userRegistration.Gender
            };

            foreach (IPasswordValidator<ApplicationUser> passwordValidator in _userManager.PasswordValidators)
            {
                var validatePassword = await passwordValidator.ValidateAsync(_userManager, user, userRegistration.Password);

                if (!validatePassword.Succeeded)
                    return StatusCode(StatusCodes.Status400BadRequest, validatePassword.Errors);
            }

            var result = await _userManager.CreateAsync(user, userRegistration.Password);

            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "User creation failed! Please check user details and try again." });

            // Verify 
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            await _userManager.AddToRoleAsync(user, UserRoles.User);

            return StatusCode(StatusCodes.Status204NoContent);
        }

        [HttpPost]
        [Route("admin/register")]
        public async Task<IActionResult> RegisterAdmin([FromBody] UserRegistration userRegistration)
        {
            var userExists = await _userManager.FindByEmailAsync(userRegistration.Email);

            if (userExists != null)
                return StatusCode(StatusCodes.Status400BadRequest, new Response { Message = $"User already register with email {userRegistration.Email}" });

            ApplicationUser user = new()
            {
                Email = userRegistration.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = userRegistration.Email,
                FirstName = userRegistration.FirstName,
                LastName = userRegistration.LastName,
                BirthDate = userRegistration.BirthDate,
                Gender = userRegistration.Gender
            };

            foreach (IPasswordValidator<ApplicationUser> passwordValidator in _userManager.PasswordValidators)
            {
                var validatePassword = await passwordValidator.ValidateAsync(_userManager, user, userRegistration.Password);

                if (!validatePassword.Succeeded)
                    return StatusCode(StatusCodes.Status400BadRequest, validatePassword.Errors);
            }

            var result = await _userManager.CreateAsync(user, userRegistration.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Message = "User creation failed! Please check user details and try again." });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            await _userManager.AddToRoleAsync(user, UserRoles.Admin);

            return StatusCode(StatusCodes.Status204NoContent);
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return token;
        }
    }
}