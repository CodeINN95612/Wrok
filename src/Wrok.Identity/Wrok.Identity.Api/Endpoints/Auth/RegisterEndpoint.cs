using MediatR;

using Wrok.Identity.Api.Common;
using Wrok.Identity.Application.Features.Auth.Register;

namespace Wrok.Identity.Api.Endpoints.Auth;

public static class RegisterEndpoint
{
    public static void MapRegisterEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", async (RegisterRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(request, ct);
            return result.Match(
                response => TypedResults.Created($"users/{response.UserId.Value}"),
                errors => CustomResults.ProblemFromErrors(errors));
        });
    }
}
