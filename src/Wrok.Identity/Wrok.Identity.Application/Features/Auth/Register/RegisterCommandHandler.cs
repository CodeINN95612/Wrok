
using ErrorOr;

using FluentValidation;

using MediatR;

using Wrok.Identity.Application.Abstractions.Common;
using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Abstractions.UnitOfWork;
using Wrok.Identity.Application.Extensions;
using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Application.Features.Auth.Register;

internal sealed class RegisterCommandHandler(
    IValidator<RegisterRequest> validator,
    IPasswordHasher passwordHasher,
    ITenantRepository tenantRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<RegisterRequest, ErrorOr<RegisterResponse>>
{

    public async Task<ErrorOr<RegisterResponse>> Handle(RegisterRequest request, CancellationToken ct)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return validationResult.Errors.ToErrorOr();
        }

        var userExists = await userRepository.IsUniqueByEmailAsync(request.Email, ct);
        if (userExists)
        {
            return RegisterErrors.EmailAlreadyInUse.ToErrorOr(ErrorType.Conflict);
        }

        var tenant = new Tenant(request.TenantName);

        var (passwordHash, salt) = passwordHasher.Hash(request.Password);
        var superAdminUser = new AdminUser(request.Email, request.FullName, passwordHash, salt, isSuper: true);

        tenant.AddAdminUser(superAdminUser);

        tenantRepository.Add(tenant);
        await unitOfWork.SaveChangesAsync(ct);

        return new RegisterResponse(superAdminUser.Id);
    }
}
