using MercuryOMS.Application.IRepository;
using MercuryOMS.Application.IServices;
using MercuryOMS.Application.UOW;
using MercuryOMS.Infrastructure.Repositories;
using MercuryOMS.Infrastructure.Services;
using MercuryOMS.Worker.PaymentConsumer;

namespace MercuryOMS.Worker
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddWorker(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddHandler();
            services.AddServices();
            return services;
        }

        private static IServiceCollection AddHandler(
            this IServiceCollection services)
        {
            services.AddScoped<IPaymentPaidHandler, UpdateOrderHandler>();
            services.AddScoped<IPaymentPaidHandler, DeductInventoryHandler>();
            services.AddScoped<IPaymentPaidHandler, SendEmailHandler>();
            services.AddScoped<IPaymentPaidHandler, CreateTransactionHandler>();
            return services;
        }

        private static IServiceCollection AddServices(
            this IServiceCollection services)
        {

            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IMessageBus, RabbitMqService>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            return services;
        }
    }
}