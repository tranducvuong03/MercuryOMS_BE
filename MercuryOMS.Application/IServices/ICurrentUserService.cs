/// <summary>
/// Note: Chỉ dùng cho các request đã được xác thực và người dùng hiện tại.
/// </summary>
public interface ICurrentUserService
{
    Guid UserId { get; }
    string Email { get; }
    string FullName { get; }
}
