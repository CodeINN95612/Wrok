using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Domain.Entities;

public record struct InvitationId(Guid Value);

public class Invitation
{
    public InvitationId Id { get; private set; }
    public TenantId InvitedToTenantId { get; private set; }
    public UserId InvitedByUserId { get; private set; }
    public UserId? CreatedUserId { get; private set; }
    public string Email { get; private set; }
    public UserRole Role { get; private set; }
    public string Code { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? AcceptedAt { get; private set; }

    public Tenant Tenant {get; private set; }
    public User InvitedByUser {get; private set; }
    public User? CreatedUser {get; private set; }

#nullable disable
    private Invitation() { } //For EF Core
#nullable enable

    public Invitation(
        AdminUser invitedBy,
        string email,
        UserRole role,
        string code)
    {
        if (role is not UserRole.ProjectManager and not UserRole.Admin)
        {
            throw new ArgumentException("Invalid role for invitation.", nameof(role));
        }

        Id = new(Guid.CreateVersion7());
        InvitedToTenantId = invitedBy.Tenant.Id;
        Tenant = invitedBy.Tenant;
        InvitedByUserId = invitedBy.Id;
        InvitedByUser = invitedBy;
        Email = email;
        Role = role;
        Code = code;
        CreatedAt = DateTime.UtcNow;
    }

    internal void AcceptBy(User createdUser)
    {
        ArgumentNullException.ThrowIfNull(createdUser, nameof(createdUser));
        if (AcceptedAt.HasValue)
        {
            throw new InvalidOperationException("Invitation has already been accepted.");
        }
        CreatedUserId = createdUser.Id;
        CreatedUser = createdUser;
        AcceptedAt = DateTime.UtcNow;
    }

}
