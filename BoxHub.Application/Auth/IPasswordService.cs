using BoxHub.Domain.Entities;

namespace BoxHub.Application.Auth;

public interface IPasswordService
{
    string HashPassword(User user, string password);
    bool VerifyPassword(User user, string hashedPassword, string providedPassword);
}

