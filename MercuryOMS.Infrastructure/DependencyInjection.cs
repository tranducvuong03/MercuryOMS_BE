using MercuryOMS.Application.Interfaces;
using MercuryOMS.Application.IServices;
using MercuryOMS.Infrastructure.Data;
using MercuryOMS.Infrastructure.Data.Interceptors;
using MercuryOMS.Infrastructure.Identity;
using MercuryOMS.Infrastructure.Repositories;
using MercuryOMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MercuryOMS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddPostgresDbContext(configuration);
            services.AddRepository();
            services.AddServices();
            services.AddJwt(configuration);
            services.AddExternal(configuration);
            return services;
        }

        private static IServiceCollection AddPostgresDbContext(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                options.UseNpgsql(configuration.GetConnectionString("MercuryOMSDatabase"));

                options.AddInterceptors(sp.GetRequiredService<AuditSaveChangesInterceptor>());
            });

            services.AddScoped<AuditSaveChangesInterceptor>();
            return services;
        }

        private static IServiceCollection AddJwt(
                    this IServiceCollection services,
                    IConfiguration configuration)
        {
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(key),

                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAuthorization();

            return services;
        }

        private static IServiceCollection AddExternal(
                    this IServiceCollection services,
                    IConfiguration configuration)
        {
            services.AddAuthentication()
                .AddGoogle("Google", options =>
                {
                    options.ClientId = configuration["Auth:Google:ClientId"];
                    options.ClientSecret = configuration["Auth:Google:ClientSecret"];
                });
            return services;
        }

        private static IServiceCollection AddRepository(
            this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            
            return services;
        }

        private static IServiceCollection AddServices(
            this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IJwtService, JwtService>();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();

            return services;
        }
    }
}