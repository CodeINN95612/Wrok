using MediatR;

using Wrok.Identity.Api.Common;
using Wrok.Identity.Api.Extensions;
using Wrok.Identity.Application.Features.Users.GetAllUsers;
using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Api.Endpoints.Users;

public static class GetAllUsersEndpoint
{
    public static IEndpointRouteBuilder MapGetAllUsersEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/users", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetAllUsersRequest(), ct);
            return result.Match(
                users => TypedResults.Ok(users),
                errors => CustomResults.ProblemFromErrors(errors));
        }).RequireAuthorizedRoles(UserRole.Admin);
        return app;
    }
}
