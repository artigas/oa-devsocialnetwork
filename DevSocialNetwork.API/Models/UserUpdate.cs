using DevSocialNetwork.API.Enums;

namespace DevSocialNetwork.API.Models
{
    public class UserUpdate
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime? BirthDate { get; set; }

        public Gender? Gender { get; set; }
    }
}
