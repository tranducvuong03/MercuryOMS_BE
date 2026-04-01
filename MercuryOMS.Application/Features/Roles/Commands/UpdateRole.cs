using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.IServices;

namespace MercuryOMS.Application.Features
{
    public record UpdateRoleCommand(string OldName, string NewName) : IRequest<Result>;

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, Result>
    {
        private readonly IRoleService _roleService;

        public UpdateRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Result> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            return await _roleService.UpdateAsync(request.OldName, request.NewName, cancellationToken);
        }
    }
}
