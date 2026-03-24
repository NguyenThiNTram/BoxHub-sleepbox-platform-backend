namespace BoxHub.Application.DTOs.Responses.Users
{
    public class UserProfileResponse
    {
        public Guid UserId { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Role { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public string UserStatus { get; set; }

        public bool IsEmailVerified { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? LastLoginAt { get; set; }

        public string AvatarUrl { get; set; }
    }
}

