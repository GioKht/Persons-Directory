using FluentValidation.Results;

namespace Persons.Directory.Application.Exceptions;

public class UnprocessableEntityException : Exception
{
    public ValidationResult ValidationResult { get; set; }

    public UnprocessableEntityException(string message, ValidationResult validationResult) : base(message)
    {
        ValidationResult = validationResult;
    }
}
