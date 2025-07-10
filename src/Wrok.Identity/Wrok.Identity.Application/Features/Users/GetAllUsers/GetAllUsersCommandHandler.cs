
using ErrorOr;

using FluentValidation;

using MediatR;

using Wrok.Identity.Application.Abstractions.Providers;
using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Dtos.Users;
using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Application.Features.Users.GetAllUsers;

internal sealed class GetAllUsersCommandHandler(
    IIdentityProvider identityProvider,
    ITenantRepository tenantRepository) : IRequestHandler<GetAllUsersRequest, ErrorOr<GetAllUsersResponse>>
{
    public async Task<ErrorOr<GetAllUsersResponse>> Handle(GetAllUsersRequest request, CancellationToken ct)
    {
        var tenantId = identityProvider.TenantId;
        if (tenantId is null)
        {
            return Error.Forbidden("GetAllUsers.NotAuthenticated", "Must be authenticated.");
        }

        var tenant = await tenantRepository.GetByIdAsync(tenantId, ct);
        if (tenant is null)
        {
            return Error.NotFound("GetAllUsers.TenantNotFound", "Tenant not found.");
        }

        var users = tenant
            .AdminUsers.Cast<User>()
            .Union(tenant.ProjectManagerUsers.Cast<User>())
            .Select(UserDto.FromUser)
            .ToList();
        var count = users.Count;
        return new GetAllUsersResponse(count, users);
    }
}