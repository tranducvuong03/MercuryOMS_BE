using MercuryOMS.API.Hubs;
using MercuryOMS.API.Middlewares;
using MercuryOMS.Application;
using MercuryOMS.Application.IServices;
using MercuryOMS.Infrastructure;
using MercuryOMS.Infrastructure.SeedData;
using MercuryOMS.Infrastructure.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// hiển thị enum dưới dạng string trong Swagger và JSON
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(
        new System.Text.Json.Serialization.JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập JWT dạng: Bearer {token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// DI
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFE", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowCredentials()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// SignalR
builder.Services.AddSignalR();
builder.Services.AddScoped<INotificationHub, NotificationHubService>();
builder.Services.AddScoped<INotificationRealtimeService, NotificationRealtimeService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Seed data
await app.SeedDataAsync();

app.UseHttpsRedirection();
app.UseCors("AllowFE");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<RequestTimeMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.MapHub<NotificationHub>("/hubs/notification");
app.MapControllers();
app.Run();
