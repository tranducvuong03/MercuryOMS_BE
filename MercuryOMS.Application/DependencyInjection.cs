using MediatR;
using FluentValidation;
using MercuryOMS.Application.Common.Behaviors;
using MercuryOMS.Application.Commons;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MercuryOMS.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddApplicationMediatR();
            services.AddApplicationAutoMapper();

            // bind CacheSettings
            services.AddCacheSetting(configuration);
            services.AddValidator();

            return services;
        }

        private static IServiceCollection AddApplicationMediatR(
            this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
                cfg.RegisterServicesFromAssembly(
                    typeof(DependencyInjection).Assembly
                ));
            return services;
        }

        private static IServiceCollection AddApplicationAutoMapper(
            this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);
            return services;
        }

        private static IServiceCollection AddCacheSetting(this IServiceCollection services,
                                                          IConfiguration configuration)
        {
            services.Configure<CacheSettings>(configuration.GetSection("CacheSettings"));
            return services;
        }

        public static IServiceCollection AddValidator(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
