using ErrorOr;

using FluentValidation.Results;

using Wrok.Identity.Application.Errors;

namespace Wrok.Identity.Application.Extensions;
internal static class ErrorExtensions
{
    public static List<Error> ToErrorOr(this List<ValidationFailure> validationFailures)
    {
        return validationFailures.Select(f => Error.Validation(f.ErrorCode, f.ErrorMessage)).ToList();
    }

    public static Error ToErrorOr(this ApplicationError appError, ErrorType type)
    {
        return Error.Custom((int)type, appError.Code, appError.Message);
    }
}
