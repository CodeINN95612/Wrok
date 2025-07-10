using MediatR;

using Wrok.Identity.Api.Common;
using Wrok.Identity.Application.Features.Invitations.AcceptInvitation;

namespace Wrok.Identity.Api.Endpoints.Invitations;

public static class AcceptInviteEndpoint
{
    public record struct AcceptInvitationEndpointRequest(
        string Email,
        string Password,
        string Fullname);

    public static IEndpointRouteBuilder MapAcceptInviteEndpoint(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/invitations/{inviteCode}/accept", async (
            string inviteCode,
            AcceptInvitationEndpointRequest endpointRequest,
            ISender sender) =>
        {
            var request = new AcceptInvitationRequest(
                inviteCode,
                endpointRequest.Email,
                endpointRequest.Password,
                endpointRequest.Fullname);
            var result = await sender.Send(request);
            return result.Match(
                response => TypedResults.Created($"/users/{response.createdUserId}"),
                errors => CustomResults.ProblemFromErrors(errors));
        })
        .WithName("AcceptInvitation")
        .WithTags("Invitations")
        .Produces<AcceptInvitationResponse>(StatusCodes.Status200OK)
        .ProducesValidationProblem()
        .Produces(StatusCodes.Status404NotFound);
        return endpoints;
    }
}
