using Microsoft.AspNetCore.Http;
using System.Security.Claims;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal User =>
        _httpContextAccessor.HttpContext?.User
        ?? throw new UnauthorizedAccessException("Không có HttpContext");

    public Guid UserId
    {
        get
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User chưa đăng nhập");

            if (!Guid.TryParse(userId, out var guid))
                throw new UnauthorizedAccessException("UserId không hợp lệ");

            return guid;
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