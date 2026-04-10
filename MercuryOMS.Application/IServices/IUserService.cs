using MercuryOMS.Application.Models.Responses;

namespace MercuryOMS.Application.IServices
{
    public interface IUserService
    {
        Task<UserResponse?> GetByIdAsync(Guid userId);
    }
}
