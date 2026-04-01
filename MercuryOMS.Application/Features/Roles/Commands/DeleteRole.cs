using MediatR;
using MercuryOMS.Application.Commons;
using MercuryOMS.Application.IServices;

namespace MercuryOMS.Application.Features
{
    public record DeleteRoleCommand(string Name) : IRequest<Result>;

    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, Result>
    {
        private readonly IRoleService _roleService;

        public DeleteRoleCommandHandler(IRoleService roleService)
        {
            _roleService = roleService;
        }

        public async Task<Result> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            return await _roleService.DeleteAsync(request.Name, cancellationToken);
        }
    }
}
