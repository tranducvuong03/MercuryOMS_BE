using MercuryOMS.Infrastructure.Data;
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
builder.Services.AddHostedService<InventoryInitConsumer>();
builder.Services.AddHostedService<PaymentPaidConsumer>();
builder.Services.AddHostedService<OutboxProcessor>();

// Services
builder.Services.AddWorker(builder.Configuration);

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("MercuryOMSDatabase"));
});

// MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

var host = builder.Build();
host.Run();