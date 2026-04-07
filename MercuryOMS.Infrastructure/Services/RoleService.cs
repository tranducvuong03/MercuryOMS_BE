using MercuryOMS.Application.Commons;
using MercuryOMS.Application.IServices;
using MercuryOMS.Application.Models;
using MercuryOMS.Domain.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MercuryOMS.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleService(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<Result> CreateAsync(string roleName, CancellationToken ct)
        {
            var exists = await _roleManager.RoleExistsAsync(roleName);
            if (exists)
                return Result.Failure(Message.RoleAlreadyExists);

            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));

            if (!result.Succeeded)
                return Result.Failure(result.Errors.First().Description);

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(string roleName, CancellationToken ct)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role == null)
                return Result.Failure(Message.RoleNotFound);

            var result = await _roleManager.DeleteAsync(role);

            if (!result.Succeeded)
                return Result.Failure(result.Errors.First().Description);

            return Result.Success();
        }

        public async Task<Result<List<RoleResponse>>> GetAllAsync(CancellationToken ct)
        {
            var roles = await _roleManager.Roles
                .Select(r => new RoleResponse
                {
                    Name = r.Name!
                })
                .ToListAsync(ct);

            return Result<List<RoleResponse>>.Success(roles);
        }

        public async Task<Result> UpdateAsync(string oldName, string newName, CancellationToken ct)
        {
            var role = await _roleManager.FindByNameAsync(oldName);
            if (role == null)
                return Result.Failure(Message.RoleNotFound);

            role.Name = newName;

            var result = await _roleManager.UpdateAsync(role);

            if (!result.Succeeded)
                return Result.Failure(result.Errors.First().Description);

            return Result.Success();
        }
    }
}