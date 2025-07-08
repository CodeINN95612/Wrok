using MediatR;

using Wrok.Identity.Api.Common;
using Wrok.Identity.Application.Features.Users;
using Wrok.Identity.Application.Features.Users.GetAllUsers;

namespace Wrok.Identity.Api.Endpoints.Users;

public static class GetAllUsersEndpoint
{
    public static void MapGetAllUsersEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/users", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetAllUsersRequest(), ct);
            return result.Match(
                users => TypedResults.Ok(users),
                errors => CustomResults.ProblemFromErrors(errors));
        });
    }
}
