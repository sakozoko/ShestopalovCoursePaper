using System;
using System.Collections.Generic;
using CourseWork.Models;

namespace CourseWork.Services;

public class TokenHandler : ITokenHandler
{
    private const int ExpirationTime = 8;
    private readonly IEncryptHandler _encryptHandler;
    private readonly ITokenCreator _tokenCreator;

    public TokenHandler(ITokenCreator tokenCreator, IEncryptHandler encryptHandler)
    {
        _encryptHandler = encryptHandler;
        _tokenCreator = tokenCreator;
    }

    public string GenerateToken(User user)
    {
        var token = _tokenCreator.GenerateToken(user.Id, user.Username!, (int)user.AccessLevel, ExpirationTime);
        var encryptedToken = _encryptHandler.Encrypt(token);
        return encryptedToken;
    }

    public User? GetUser(string token)
    {
        try
        {
            var decryptedToken = _encryptHandler.Decrypt(token);
            var tokenParts = decryptedToken.Split(";");
            if (tokenParts.Length != 4) return null;
            var id = int.Parse(tokenParts[0]);
            var username = tokenParts[1];
            var accessLevel = int.Parse(tokenParts[2]);

            var user = new User { Id = id, Username = username, AccessLevel = (AccessLevel)accessLevel };
            return user;
        }
        catch
        {
            return null;
        }
    }

    public string? RenewToken(string token)
    {
        try
        {
            var decryptedToken = _encryptHandler.Decrypt(token);
            var tokenParts = decryptedToken.Split(";");
            if (tokenParts.Length != 4) return null;
            var id = int.Parse(tokenParts[0]);
            var username = tokenParts[1];
            var accessLevel = int.Parse(tokenParts[2]);

            var newToken = _tokenCreator.GenerateToken(id, username, accessLevel, ExpirationTime);
            return _encryptHandler.Encrypt(newToken);
        }
        catch
        {
            return null;
        }
    }

    public bool ValidateToken(string token)
    {
        try
        {
            var decryptedToken = _encryptHandler.Decrypt(token);
            var tokenParts = decryptedToken.Split(";");
            if (tokenParts.Length != 4) return false;
            var expirationTime = DateTime.Parse(tokenParts[3]);
            if (expirationTime < DateTime.Now) return false;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool IsTokenExpired(string token)
    {
        try
        {
            var decryptedToken = _encryptHandler.Decrypt(token);
            var tokenParts = decryptedToken.Split(";");
            if (tokenParts.Length != 4) return false;
            if (string.IsNullOrWhiteSpace(tokenParts[3])) return false;
            var expirationTime = DateTime.Parse(tokenParts[3]);
            return expirationTime < DateTime.Now;
        }
        catch
        {
            return false;
        }
    }
}