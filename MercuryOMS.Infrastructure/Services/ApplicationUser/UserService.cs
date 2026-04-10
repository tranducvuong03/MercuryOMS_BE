using MercuryOMS.Application.IServices;
using MercuryOMS.Application.Models.Responses;
using MercuryOMS.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace MercuryOMS.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<UserResponse?> GetByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null) return null;

            return new UserResponse
            {
                Id = Guid.Parse(user.Id),
                Email = user.Email!,
                FullName = user.FullName
            };
        }
    }
}
