using ErrorOr;

using Microsoft.AspNetCore.Mvc;

namespace Wrok.Identity.Api.Common;

internal static class CustomResults
{
    public static IResult ProblemFromErrors(List<Error> errors)
    {
        if (errors.Any(e => e.Type != ErrorType.Validation))
        {
            // For non-validation errors, return a 500 Internal Server Error Problem Details
            // It will contain all errores as metadata
            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An unexpected error occurred.",
                Detail = "Please try again later."
            };
            problemDetails.Extensions.Add("errors", errors.Select(s => new
            {
                Code = s.Code,
                Description = s.Description
            }));
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
}
