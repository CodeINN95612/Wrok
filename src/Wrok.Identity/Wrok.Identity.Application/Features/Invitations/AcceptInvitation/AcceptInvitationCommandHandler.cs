using ErrorOr;

using FluentValidation;

using MediatR;

using Wrok.Identity.Application.Abstractions.Common;
using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Abstractions.UnitOfWork;
using Wrok.Identity.Application.Extensions;
using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Application.Features.Invitations.AcceptInvitation;

internal sealed class AcceptInvitationCommandHandler(
    IValidator<AcceptInvitationRequest> validator,
    ITenantRepository tenantRepository,
    IInvitationRepository invitationRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork) : IRequestHandler<AcceptInvitationRequest, ErrorOr<AcceptInvitationResponse>>
{
    public async Task<ErrorOr<AcceptInvitationResponse>> Handle(AcceptInvitationRequest request, CancellationToken ct)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrorOr();
        }

        var invitation = await invitationRepository.GetByCodeAsync(request.Code, ct);
        if (invitation is null)
        {
            return AcceptInvitationErrors.InvitationNotFound.ToErrorOr(ErrorType.NotFound);
        }

        if (invitation.AcceptedAt.HasValue)
        {
            return AcceptInvitationErrors.InvitationAlreadyAccepted.ToErrorOr(ErrorType.Conflict);
        }

        var (passwordHash, salt) = passwordHasher.Hash(request.Password);

        User user = invitation.Role switch
        {
            Domain.Enums.UserRole.Admin => new AdminUser(request.Email, request.Fullname, passwordHash, salt),
            Domain.Enums.UserRole.ProjectManager => new ProjectManagerUser(request.Email, request.Fullname, passwordHash, salt),
            _ => throw new InvalidOperationException($"This should never happen")
        };

        invitation.Tenant.JoinByInvite(user, request.Code);

        tenantRepository.Update(invitation.Tenant);

        await unitOfWork.SaveChangesAsync(ct);

        return new AcceptInvitationResponse(user.Id);
    }
}
