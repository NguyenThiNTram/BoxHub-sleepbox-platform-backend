namespace BoxHub.Application.Auth;

public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string inputPassword);
}