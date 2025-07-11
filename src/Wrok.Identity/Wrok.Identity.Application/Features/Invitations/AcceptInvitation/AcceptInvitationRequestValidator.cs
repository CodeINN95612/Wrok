using FluentValidation;

using Wrok.Identity.Application.Abstractions.Common;

namespace Wrok.Identity.Application.Features.Invitations.AcceptInvitation;

internal sealed class AcceptInvitationRequestValidator : AbstractValidator<AcceptInvitationRequest>
{
    public AcceptInvitationRequestValidator(IRegexValidator regexValidator)
    {
        var codeRequired = AcceptInvitationErrors.CodeRequired;
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithErrorCode(codeRequired.Code)
            .WithMessage(codeRequired.Message);

        var emailRequired = AcceptInvitationErrors.EmailRequired;
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithErrorCode(emailRequired.Code)
            .WithMessage(emailRequired.Message);

        var emailInvalid = AcceptInvitationErrors.EmailInvalid;
        RuleFor(x => x.Email)
            .Must(regexValidator.IsValidEmail)
            .WithErrorCode(emailInvalid.Code)
            .WithMessage(emailInvalid.Message);

        var passwordRequired = AcceptInvitationErrors.PasswordRequired;
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithErrorCode(passwordRequired.Code)
            .WithMessage(passwordRequired.Message);

        var passwordInvalid = AcceptInvitationErrors.PasswordInvalid;
        RuleFor(x => x.Password)
            .Must(regexValidator.IsValidPassword)
            .WithErrorCode(passwordInvalid.Code)
            .WithMessage(passwordInvalid.Message);

        var fullnameRequired = AcceptInvitationErrors.FullnameRequired;
        RuleFor(x => x.Fullname)
            .NotEmpty()
            .WithErrorCode(fullnameRequired.Code)
            .WithMessage(fullnameRequired.Message);
    }
}
