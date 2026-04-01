using MercuryOMS.Application.Commons;
using MercuryOMS.Application.IServices;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace MercuryOMS.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;

        public AuthService(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
        }

        public async Task<Result> LoginAsync(string email, string password, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result.Failure(Message.AuthEmailNotFound);

            var result = await _signInManager.CheckPasswordSignInAsync(
                user, password, lockoutOnFailure: true);

            if (result.IsLockedOut) return Result.Failure(Message.AuthLockedOut);
            if (result.IsNotAllowed) return Result.Failure(Message.AuthNotAllowed);
            if (!result.Succeeded) return Result.Failure(Message.AuthInvalidPassword);

            return Result.Success(Message.LoginSuccess);
        }

        public async Task<Result> RegisterAsync(string email, string password, string fullName, CancellationToken ct)
        {
            var existing = await _userManager.FindByEmailAsync(email);
            if (existing != null)
                return Result.Failure(Message.AuthEmailAlreadyExists);

            var user = new ApplicationUser
            {
                FullName = fullName,
                UserName = email,
                Email = email,
            };

            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
                return Result.Failure(createResult.Errors.First().Description);

            var roleExists = await _roleManager.RoleExistsAsync(Role.Member);
            if (!roleExists)
                return Result.Failure(Message.RoleNotFound);

            var addRoleResult = await _userManager.AddToRoleAsync(user, Role.Member);
            if (!addRoleResult.Succeeded)
                return Result.Failure(addRoleResult.Errors.First().Description);

            await SendConfirmEmailAsync(user);

            return Result.Success(Message.RegisterSuccess);
        }

        public async Task<Result> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result.Failure(Message.UserNotFound);

            if (user.EmailConfirmed)
                return Result.Failure(Message.EmailAlreadyConfirmed);

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (!result.Succeeded)
                return Result.Failure(Message.EmailConfirmInvalidToken);

            return Result.Success(Message.EmailConfirmed);
        }

        public async Task<Result> ResendConfirmEmailAsync(string email, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result.Failure(Message.AuthEmailNotFound);

            if (user.EmailConfirmed)
                return Result.Failure(Message.EmailAlreadyConfirmed);

            await SendConfirmEmailAsync(user);

            return Result.Success(Message.EmailConfirmSent);
        }

        private async Task SendConfirmEmailAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmLink =
                $"https://localhost:7245/api/auth/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            await _emailService.SendAsync(
                user.Email!,
                "Confirm your email",
                $"Click để xác nhận: <a href='{confirmLink}'>Confirm Email</a>");
        }
    }
}