namespace CourseWork.Services;

public interface ITokenCreator
{
    string GenerateToken(int id, string username, int accessLevel, int expirationTime);
}