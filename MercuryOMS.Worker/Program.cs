using MercuryOMS.Application.IServices;
using MercuryOMS.Infrastructure.Services;
using MercuryOMS.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<PaymentPaidConsumer>();

builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var host = builder.Build();
host.Run();
