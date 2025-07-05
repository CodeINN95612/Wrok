using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Domain.Entities;
public sealed class FreelancerUser : User
{
    public string Title { get; private set; }
    public string Bio { get; private set; }

#nullable disable
    private FreelancerUser() { } // For EF Core
#nullable enable

    public FreelancerUser(string email, string fullName, string passwordHash, string salt, string title, string bio)
        : base(email, fullName, passwordHash, salt, UserRole.Freelancer)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
        ArgumentException.ThrowIfNullOrWhiteSpace(bio, nameof(bio));
        Title = title;
        Bio = bio;
    }
    public void UpdateTitle(string title)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title, nameof(title));
        Title = title;
    }
    public void UpdateBio(string bio)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(bio, nameof(bio));
        Bio = bio;
    }
}
