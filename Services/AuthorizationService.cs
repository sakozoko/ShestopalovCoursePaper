using CourseWork.Models;

namespace CourseWork.Services;

public class AuthorizationService
{
    private readonly AuthenticationService _authenticationService;

    public AuthorizationService(AuthenticationService authenticationService, ITokenHandler tokenHandler)
    {
        _authenticationService = authenticationService;
    }

    public bool Authorize(string username, string password, AccessLevel accessLevel = 0)
    {
        var user = _authenticationService.GetUser(username, password);
        return Authorize(user, accessLevel);
    }

    public bool Authorize(User? user, AccessLevel accessLevel = 0)
    {
        if (user != null)
            if (user.AccessLevel >= accessLevel)
                return true;
        return false;
    }

    public bool Authorize(string? token, AccessLevel accessLevel = 0)
    {
        var user = _authenticationService.GetUser(token);
        return Authorize(user, accessLevel);
    }
}