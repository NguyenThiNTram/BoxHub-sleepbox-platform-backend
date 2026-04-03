namespace BoxHub.Application.DTOs.Requests.Users;

public sealed class UpdateUserProfileRequest
{
    public string? Username { get; set; }
    public string? Phone { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Gender { get; set; }
    public DateOnly? DateOfBirth { get; set; }
}
