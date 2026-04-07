using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.IServices;
using MercuryOMS.Application.Models;

namespace MercuryOMS.Application.Features
{
    public record GetAllRolesQuery() : IRequest<Result<List<RoleResponse>>>;

    public class GetAllRolesQueryHandler : IRequestHandler<GetAllRolesQuery, Result<List<RoleResponse>>>
    {
        private readonly IRoleService _roleService;

        public GetAllRolesQueryHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Result<List<RoleResponse>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            return await _roleService.GetAllAsync(cancellationToken);
        }
    }
}
