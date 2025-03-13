using FluentValidation.Results;

namespace Saylo.Centrex.Application.Exceptions;

public class ValidationException : Exception
{
    public List<ValidationFailure> Errors { get; }

    public ValidationException(List<ValidationFailure> errors)
    {
        Errors = errors;
    }
    public ValidationException(string message)  : this (message, new List<ValidationFailure>()) 
    {
    }
    public ValidationException(string message, List<ValidationFailure> errors): base(message)
    {
        if (!errors.Any())
        {
            errors = new List<ValidationFailure> { new("Validation Error", message) };
        }

        Errors = errors;
    }

    public ValidationException(string message, Exception innerException, List<ValidationFailure> errors)
        : base(message, innerException)
    {
        Errors = errors;
    }
}