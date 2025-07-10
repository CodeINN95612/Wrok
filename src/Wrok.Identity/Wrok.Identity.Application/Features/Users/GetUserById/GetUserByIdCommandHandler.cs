
using ErrorOr;

using MediatR;

using Wrok.Identity.Application.Abstractions.Providers;
using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Dtos.Users;
using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Application.Features.Users.GetById;

public sealed record GetUserByIdRequest(Guid UserId) : IRequest<ErrorOr<GetUserByIdResponse>>;
public sealed record GetUserByIdResponse(UserDto User);

internal sealed class GetUserByIdCommandHandler(
    IIdentityProvider identityProvider,
    ITenantRepository tenantRepository) : IRequestHandler<GetUserByIdRequest, ErrorOr<GetUserByIdResponse>>
{
    public async Task<ErrorOr<GetUserByIdResponse>> Handle(GetUserByIdRequest request, CancellationToken ct)
    {
        var tenantId = identityProvider.TenantId;
        if (tenantId is null)
        {
            return Error.Forbidden("GetUserById.NotAuthenticated");
        }

        var tenant = await tenantRepository.GetByIdAsync(tenantId, ct);
        if (tenant is null)
        {
            return Error.NotFound(
                code: "GetUserById.TenantNotFound",
                description: $"Tenant not found.");
        }

        var id = new UserId(request.UserId);
        var user = tenant.GetUser(id);

        if (user is null)
        {
            return Error.NotFound(
                code: "GetUserById.UserNotFound",
                description: $"User with ID {request.UserId} not found.");
        }

        var userDto = UserDto.FromUser(user);
        return new GetUserByIdResponse(userDto);
    }
}
