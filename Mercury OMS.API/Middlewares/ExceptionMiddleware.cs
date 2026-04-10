using FluentValidation;
using MercuryOMS.Domain.Exceptions;

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
            catch (ValidationException ex) // xử lý lỗi validation từ FluentValidation
            {
                context.Response.StatusCode = 400;

                var errors = ex.Errors.Select(x => x.ErrorMessage);

                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    errorCode = "VALIDATION_ERROR",
                    errors
                });
            }
            catch (AppException ex) // xử lý custom exceptions
            {
                context.Response.StatusCode = ex.StatusCode;

                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    errorCode = ex.ErrorCode,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 500;

                await context.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    errorCode = "INTERNAL_ERROR",
                    message = "Internal Server Error"
                });
            }
        }
    }
}
