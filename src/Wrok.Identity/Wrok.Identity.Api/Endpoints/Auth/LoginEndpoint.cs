using MediatR;

using Wrok.Identity.Api.Common;
using Wrok.Identity.Application.Features.Auth.Login;

namespace Wrok.Identity.Api.Endpoints.Auth;

public static class LoginEndpoint
{
    public static void MapLoginEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", async (LoginRequest request, ISender sender) =>
        {
            var result = await sender.Send(request);
            return result.Match(
                success => TypedResults.Ok(success),
                errors => CustomResults.ProblemFromErrors(errors));
        })
        .WithName("Login")
        .WithTags("Auth")
        .Produces<LoginResponse>()
        .ProducesProblem(StatusCodes.Status400BadRequest);
    }
}
