using FluentValidation;
using MediatR;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;

namespace Hello100Admin.BuildingBlocks.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (_validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();
                if (failures.Count != 0)
                {
                    var messages = failures.Select(f => f.ErrorMessage).ToArray();
                    // Validation 예외는 명시적 ValidationException을 던져 미들웨어에서 적절히 매핑되도록 함
                    throw new Errors.ValidationException(
                        (int)GlobalErrorCode.ValidationError,
                        GlobalErrorCode.ValidationError.ToString(),
                        //GlobalErrorCode.ValidationError.ToError().Message,
                        string.Join("; ", messages),
                        messages);
                }
        }
        return await next();
    }
}
