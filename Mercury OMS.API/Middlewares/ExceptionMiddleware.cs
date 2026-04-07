using FluentValidation;
using System.Net;
using System.Text.Json;

namespace MercuryOMS.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

                var errors = ex.Errors.Select(x => x.ErrorMessage);

                var response = JsonSerializer.Serialize(new
                {
                    success = false,
                    errors
                });

                await context.Response.WriteAsync(response);
            }
        }
    }
}
