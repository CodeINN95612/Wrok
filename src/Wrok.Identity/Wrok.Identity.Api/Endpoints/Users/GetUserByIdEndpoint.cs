using MediatR;

using Wrok.Identity.Api.Common;
using Wrok.Identity.Application.Features.Users.GetById;

namespace Wrok.Identity.Api.Endpoints.Users;

internal static class GetUserByIdEndpoint
{
    public static IEndpointRouteBuilder MapGetUserByIdEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet(
            "/users/{userId:guid}",
            async (Guid userId, ISender sender, CancellationToken ct) =>
            {
                var request = new GetUserByIdRequest(userId);
                var result = await sender.Send(request, ct);
                return result.Match(
                    response => TypedResults.Ok(response.User),
                    errors => CustomResults.ProblemFromErrors(errors));
            })
            .WithName("GetUserById")
            .WithTags("Users");
        return endpoints;
    }
}
