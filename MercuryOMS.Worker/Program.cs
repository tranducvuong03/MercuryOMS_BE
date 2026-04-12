using MercuryOMS.Application.IServices;
using MercuryOMS.Infrastructure.Data;
using MercuryOMS.Infrastructure.Services;
using MercuryOMS.Worker;
using MercuryOMS.Worker.OutboxProcessor;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

// Load config
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Hosted Services
builder.Services.AddHostedService<PaymentPaidConsumer>();
builder.Services.AddHostedService<OutboxProcessor>();

// Services
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<IMessageBus, RabbitMqService>();

builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("MercuryOMSDatabase"));
});

var host = builder.Build();
host.Run();