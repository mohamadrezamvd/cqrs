using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Ardalis.Result;
namespace LendTech.Application.Behaviors;
/// <summary>
/// Behavior اعتبارسنجی برای MediatR
/// </summary>
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
where TRequest : IRequest<TResponse>
{
private readonly IEnumerable<IValidator<TRequest>> _validators;
public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
{
    _validators = validators;
}

public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
{
    if (!_validators.Any())
    {
        return await next();
    }

    var context = new ValidationContext<TRequest>(request);
    var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
    var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

    if (failures.Count != 0)
    {
        // اگر TResponse از نوع Result باشد
        if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            var resultType = typeof(TResponse).GetGenericArguments()[0];
            var invalidMethod = typeof(Result<>).MakeGenericType(resultType).GetMethod("Invalid", new[] { typeof(ValidationError[]) });
            
            var validationErrors = failures.Select(f => new ValidationError
            {
                Identifier = f.PropertyName,
                ErrorMessage = f.ErrorMessage
            }).ToArray();

            return (TResponse)invalidMethod!.Invoke(null, new object[] { validationErrors })!;
        }

        throw new ValidationException(failures);
    }

    return await next();
}
}
