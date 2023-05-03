using System;
using System.IO;
using CourseWork.Models;

namespace CourseWork.Services;

public class AuthenticationService
{
    private const string UsersCredentialsPath = "users.database";
    private const int MaxUsers = 14;
    private readonly IHasher _hasher;
    private readonly ITokenHandler _tokenHandler;
    private int _id;

    public AuthenticationService(ITokenHandler tokenHandler, IHasher hasher)
    {
        _hasher= hasher;
        _tokenHandler = tokenHandler;
        Init();
    }

    private void Init()
    {
        var str = File.Open(UsersCredentialsPath, FileMode.OpenOrCreate);
        var reader = new StreamReader(str);
        var line = reader.ReadLine();
        while (line != null)
        {
            _id++;
            line = reader.ReadLine();
        }

        reader.Close();
        str.Close();
        str = File.Open(UsersCredentialsPath, FileMode.Append);
        if (_id == 0)
        {
            var writer = new StreamWriter(str);
            var salt = Guid.NewGuid().ToString();
            var passwordHash = _hasher.Hash("admin", salt);
            writer.WriteLine($"{++_id} admin {passwordHash} {(int)AccessLevel.E} {salt}");
            writer.Close();
        }

        str.Close();
    }
    private bool CheckIfLineContainsUsernameAndPassword(string line, string username, string password){
        var credentials = line.Split(' ');
        var salt = credentials[4];
        var hashedPassword = _hasher.Hash(password, salt);
        if (credentials[1] == username)
            if (credentials[2] == hashedPassword)
            {
                return true;
            }
        return false;
    }

    public bool Login(string username, string password)
    {
        var result = false;
        var str = File.Open(UsersCredentialsPath, FileMode.OpenOrCreate);
        var reader = new StreamReader(str);
        var line = reader.ReadLine();
        while (line != null)
        {
            if (CheckIfLineContainsUsernameAndPassword(line, username, password))
            {
                result = true;
                break;
            }

            line = reader.ReadLine();
        }

        reader.Close();
        str.Close();
        return result;
    }

    public void Logout()
    {
        Client.Token = null;
    }


    public string? GetAccessToken(string username, string password)
    {
        var user = GetUser(username, password);
        if (user != null) return _tokenHandler.GenerateToken(user);
        return null;
    }
    public User? GetUser(string? token)
    {
        return token is not null ? _tokenHandler.GetUser(token) : default;
    }
    public bool IsTokenExpired(string token)
    {
        return _tokenHandler.IsTokenExpired(token);
    }

    public User? GetUser(string username, string password)
    {
        if(Login(username,password)){
            var str = File.Open(UsersCredentialsPath, FileMode.OpenOrCreate);
            var reader = new StreamReader(str);
            var line = reader.ReadLine();
            while (line != null)
            {
                if (CheckIfLineContainsUsernameAndPassword(line, username, password))
                {
                    var credentials = line.Split(' ');
                    var id = int.Parse(credentials[0]);
                    var accessLevel = (AccessLevel)int.Parse(credentials[3]);
                    var user = new User(){Id = id, Username = username, AccessLevel = accessLevel};
                    reader.Close();
                    str.Close();
                    return user;
                }

                line = reader.ReadLine();
            }

            reader.Close();
            str.Close();
        }
        return null;
    }

    public void Register(string username, string password)
    {
        if (_id >= MaxUsers)
            throw new Exception("Max users reached");
        var writer = File.AppendText(UsersCredentialsPath);
        var salt = Guid.NewGuid().ToString();
        var passwordHash = _hasher.Hash(password, salt);
        writer.WriteLine($"{++_id} {username} {passwordHash} {(int)AccessLevel.C} {salt}");
        writer.Close();
    }

    public string? RenewToken(string token)
    {
        return _tokenHandler.RenewToken(token);
    }
}