
using ErrorOr;

using FluentValidation;

using MediatR;

using Wrok.Identity.Application.Abstractions.Common;
using Wrok.Identity.Application.Abstractions.Providers;
using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Abstractions.UnitOfWork;
using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Application.Features.Invitations.Invite;

internal sealed class InviteCommandHandler(
    IValidator<InviteRequest> validator,
    IIdentityProvider identityProvider,
    ITokenGenerator tokenGenerator,
    ITenantRepository tenantRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<InviteRequest, ErrorOr<InviteResponse>>
{
    public async Task<ErrorOr<InviteResponse>> Handle(InviteRequest request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request); 
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .Select(error => Error.Validation($"Invite.{error.PropertyName}.{error.ErrorCode}", error.ErrorMessage))
                .ToList();
            return errors;
        }

        UserRole? role = Enum.TryParse<UserRole>(request.Role, true, out var userRole) ? userRole : null;
        if (role is null or UserRole.Freelancer)
        {
            return Error.Validation("Invite.InvalidRole", "Invalid user role specified.");
        }

        var tenantId = identityProvider.TenantId!;
        var userId = identityProvider.UserId!;

        var tenant = await tenantRepository.GetByIdAsync(tenantId, cancellationToken);
        if (tenant is null)
        {
            return Error.NotFound("Invite.TenantNotFound", "Tenant not found.");
        }

        var user = tenant.GetAdminUser(userId);
        if (user is null)
        {
            return Error.NotFound("Invite.UserNotFound", "User not found in the tenant as an admin.");
        }

        var generateInviteCode = tokenGenerator.Generate(10);
        var invitation = new Invitation(user, request.Email, role.Value, generateInviteCode);

        tenant.Invite(invitation);
        tenantRepository.Update(tenant);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new InviteResponse(invitation.Id);
    }
}