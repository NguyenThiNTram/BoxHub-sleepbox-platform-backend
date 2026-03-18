namespace BoxHub.Application.Interfaces.Services;

public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPassword, string inputPassword);
}