using DevSocialNetwork.API.Enums;

namespace DevSocialNetwork.API.Models
{
    public class UserProfileResponse
    {
        public UserProfileResponse(ApplicationUser applicationUser)
        {
            this.Id = applicationUser.Id;
            this.FirstName = applicationUser.FirstName;
            this.LastName = applicationUser.LastName;
            this.Email = applicationUser.Email;
            this.BirthDate = applicationUser.BirthDate;
            this.Gender = applicationUser.Gender;
        }

        public string Id { get; set; }

        public string? Email { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime? BirthDate { get; set; }
        public Gender? Gender { get; set; }
    }
}
