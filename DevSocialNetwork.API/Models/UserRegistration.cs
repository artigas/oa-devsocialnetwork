using DevSocialNetwork.API.Enums;
using System.ComponentModel.DataAnnotations;

namespace DevSocialNetwork.API.Models
{
    public class UserRegistration
    {
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "User Name is required")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "User Name is required")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        public DateTime? BirthDate { get; set; }

        public Gender? Gender { get; set; }
    }
}
