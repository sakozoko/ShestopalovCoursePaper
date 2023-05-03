using CourseWork.Models;

namespace CourseWork.Services;

public interface ITokenHandler
{
    string GenerateToken(User user);
    bool ValidateToken(string token);
    User? GetUser(string token);
    string? RenewToken(string token);
    bool IsTokenExpired(string token);
}