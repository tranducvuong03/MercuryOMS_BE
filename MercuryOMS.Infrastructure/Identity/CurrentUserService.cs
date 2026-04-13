using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?.User
                ?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(userId, out var guid))
                return guid;

            return null;
        }
    }

    public string Email
    {
        get
        {
            var email = User.FindFirstValue(ClaimTypes.Email);

            if (string.IsNullOrEmpty(email))
                throw new UnauthorizedAccessException("Không có email");

            return email;
        }
    }

    public string FullName
    {
        get
        {
            var fullName = User.FindFirstValue("fullName");

            if (string.IsNullOrEmpty(fullName))
                throw new UnauthorizedAccessException("Không có fullName");

            return fullName;
        }
    }
}