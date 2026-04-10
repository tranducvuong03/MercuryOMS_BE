using MercuryOMS.Application.Commons;
using MercuryOMS.Application.IServices;
using MercuryOMS.Domain.Constants;
using MercuryOMS.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace MercuryOMS.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;

        public AuthService(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IEmailService emailService,
            IConfiguration configuration,
            IJwtService jwtService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _emailService = emailService;
            _configuration = configuration;
            _jwtService = jwtService;
        }

        public async Task<Result<string>> LoginAsync(string email, string password, CancellationToken ct)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Result<string>.Failure(Message.AuthEmailNotFound);

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);

            if (result.IsLockedOut)
                return Result<string>.Failure(Message.AuthLockedOut);

            if (result.IsNotAllowed)
                return Result<string>.Failure(Message.AuthNotAllowed);

            if (!result.Succeeded)
                return Result<string>.Failure(Message.AuthInvalidPassword);

            var token = await _jwtService.GenerateTokenAsync(user);

            return Result<string>.Success(token);
        }

        public async Task<Result<string>> ExternalLoginCallbackAsync()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
                return Result<string>.Failure(Message.ExternalLoginProviderNotFound);

            var signInResult = await _signInManager.ExternalLoginSignInAsync(
                info.LoginProvider,
                info.ProviderKey,
                isPersistent: false);

            ApplicationUser user;

            if (signInResult.Succeeded)
            {
                user = await _userManager.FindByLoginAsync(
                    info.LoginProvider,
                    info.ProviderKey);
            }
            else
            {
                var email = info.Principal.FindFirst(ClaimTypes.Email)?.Value;

                if (email == null)
                    return Result<string>.Failure(Message.ExternalEmailNotFound);

                user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    user = new ApplicationUser
                    {
                        FullName = info.Principal.FindFirst(ClaimTypes.Name)?.Value ?? email,
                        Email = email,
                        UserName = email,
                        EmailConfirmed = true
                    };

                    await _userManager.CreateAsync(user);
                    await _userManager.AddToRoleAsync(user, Role.Member);
                }

                await _userManager.AddLoginAsync(user, info);
            }

            var token = await _jwtService.GenerateTokenAsync(user);

            return Result<string>.Success(token);
        }

        public async Task<Result> RegisterAsync(
            string email,
            string password,
            string fullName,
            CancellationToken ct)
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

        private async Task SendConfirmEmailAsync(ApplicationUser user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var baseUrl = _configuration["App:BaseUrl"];

            var confirmLink =
                $"{baseUrl}/api/auth/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";

            await _emailService.SendAsync(
                user.Email!,
                "CONFIRM YOUR EMAIL - MercuryOMS",
                EmailTemplates.ConfirmEmail(confirmLink));
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
    }
}