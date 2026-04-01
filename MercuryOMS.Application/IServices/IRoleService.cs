using MercuryOMS.Application.Commons;
using MercuryOMS.Application.Models.Responses;

namespace MercuryOMS.Application.IServices
{
    public interface IRoleService
    {
        Task<Result> CreateAsync(string roleName, CancellationToken ct);
        Task<Result> DeleteAsync(string roleName, CancellationToken ct);
        Task<Result<List<RoleResponse>>> GetAllAsync(CancellationToken ct);
        Task<Result> UpdateAsync(string oldName, string newName, CancellationToken ct);
    }
}
