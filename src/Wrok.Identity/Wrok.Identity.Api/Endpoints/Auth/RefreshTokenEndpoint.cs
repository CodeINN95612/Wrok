using MediatR;

using Wrok.Identity.Api.Common;
using Wrok.Identity.Application.Features.Auth.RefreshTokens;

namespace Wrok.Identity.Api.Endpoints.Auth;

public static class RefreshTokenEndpoint
{
    public static IEndpointRouteBuilder MapRefreshTokenEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/refresh-token", async (
            RefreshTokenRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var result = await sender.Send(request, ct);
            return result.Match(
                response => TypedResults.Ok(response),
                errors => CustomResults.ProblemFromErrors(errors));
        })
        .WithName("RefreshToken")
        .Produces<RefreshTokenResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status404NotFound)
        .Produces(StatusCodes.Status409Conflict);
        return app;
    }
}
