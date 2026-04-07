using FluentValidation;
using MediatR;
using MercuryOMS.Application.Commons;
using System.Reflection;

namespace MercuryOMS.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse>
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var failures = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken))
            );

            var errors = failures
                .SelectMany(r => r.Errors)
                .Where(f => f != null)
                .ToList();

            if (errors.Any())
            {
                var errorMessage = string.Join(", ", errors.Select(x => x.ErrorMessage));

                var responseType = typeof(TResponse);

                if (responseType.IsGenericType &&
                    responseType.GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var genericType = responseType.GetGenericArguments()[0];

                    var failureMethod = typeof(Result<>)
                        .MakeGenericType(genericType)
                        .GetMethod("Failure", BindingFlags.Public | BindingFlags.Static, new[] { typeof(string) });

                    var result = failureMethod!.Invoke(null, new object[] { errorMessage });

                    return (TResponse)result!;
                }

                if (responseType == typeof(Result))
                {
                    return (TResponse)(object)Result.Failure(errorMessage);
                }

                throw new ValidationException(errors);
            }

            return await next();
        }
    }
}