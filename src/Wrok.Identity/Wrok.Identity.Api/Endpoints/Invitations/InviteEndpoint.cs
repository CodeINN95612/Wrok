using MediatR;

using Microsoft.AspNetCore.Mvc;

using Wrok.Identity.Api.Common;
using Wrok.Identity.Api.Extensions;
using Wrok.Identity.Application.Features.Invitations.Invite;
using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Api.Endpoints.Invitations;

public static class InviteEndpoint
{
    public static IEndpointRouteBuilder MapInviteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/invitations", async (InviteRequest request, ISender sender) =>
        {
            var result = await sender.Send(request);
            return result.Match(
                response => TypedResults.Created($"/invitations/{response.InviteId.Value}"),
                errors => CustomResults.ProblemFromErrors(errors));
        }).RequireAuthorizedRoles(UserRole.Admin)
        .WithName("InviteUser")
        .WithTags("Invitations")
        .Produces<InviteResponse>(StatusCodes.Status200OK)
        .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
        .RequireAuthorization();
        return endpoints;
    }
}
