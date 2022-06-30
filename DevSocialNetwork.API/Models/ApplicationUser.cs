using DevSocialNetwork.API.Enums;
using Microsoft.AspNetCore.Identity;

namespace DevSocialNetwork.API.Models
{
    public class ApplicationUser : IdentityUser
    {

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }

        public Gender? Gender { get; set; }
    }
}
