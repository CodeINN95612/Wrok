using ErrorOr;

using Microsoft.AspNetCore.Mvc;

namespace Wrok.Identity.Api.Common;

internal static class CustomResults
{
    public static IResult ProblemFromErrors(List<Error> errors)
    {
        if (errors.Any(e => e.Type != ErrorType.Validation))
        {
            // For non-validation errors, return the first error
            var error = errors.First(e => e.Type != ErrorType.Validation);
            var problemDetails = new ProblemDetails
            {
                Status = GetStatusCode(error.Type),
                Title = error.Code,
                Detail = error.Description,
            };
            return TypedResults.Problem(problemDetails);
        }

        // For validation errors, return a 400 Bad Request Problem Details
        var validationProblemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Error",
            Detail = "One or more validation errors have occurred."
        };
        validationProblemDetails.Extensions.Add("errors", errors.Select(s => new
        {
            Code = s.Code,
            Description = s.Description
        }));

        return TypedResults.Problem(validationProblemDetails);
    }

    private static int GetStatusCode(ErrorType type)
    {
        return type switch
        {
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status500InternalServerError
        };
    }
}
