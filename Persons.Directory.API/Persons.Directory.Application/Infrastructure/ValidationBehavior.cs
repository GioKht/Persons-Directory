using FluentValidation;
using FluentValidation.Results;
using MediatR;
using Persons.Directory.Application.Exceptions;
using Serilog;
using System.Linq.Expressions;

namespace Persons.Directory.Application.Infrastructure
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, new()
    {
        public ValidationBehavior()
        {
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var typeToCheck = typeof(AbstractValidator<>).MakeGenericType(request.GetType());

            var validatorType = request.GetType().Assembly.GetTypes()
                .Where(x => typeToCheck.IsAssignableFrom(x))
                .FirstOrDefault();

            if (validatorType != null)
            {
                var @delegate = Expression.Lambda(Expression.New(validatorType)).Compile();
                var validatorInstance = @delegate.DynamicInvoke();

                var validateMethod = validatorInstance.GetType().GetMethod("Validate", new[] { request.GetType() });
                var result = validateMethod.Invoke(validatorInstance, new object[] { request }) as ValidationResult;

                if (!result.IsValid)
                {
                    var exception = new UnprocessableEntityException($"Validation error: '{result}'", result);

                    Log.Error(exception, exception.Message);

                    throw exception;
                }
            }

            var response = await next();

            return response;
        }
    }
}
