using System;

namespace CourseWork.Services;

public class TokenCreator : ITokenCreator
{
    public TokenCreator()
    {
    }

    public string GenerateToken(int id, string username, int accessLevel, int expirationTime)
    {
        var token = $"{id};{username};{accessLevel};{DateTime.Now.AddSeconds(expirationTime)}";
        return token;
    }
}