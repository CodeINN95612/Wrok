using Wrok.Identity.Domain.Entities;
using Wrok.Identity.Domain.Enums;

namespace Wrok.Identity.Domain.Tests.Entities;
[TestFixture]
public class FreelancerUserTests
{
    [Test]
    public void Constructor_ShouldInitializeProperties()
    {
        // Arrange
        var email = "freelancer@example.com";
        var fullName = "Freelancer User";
        var passwordHash = "hash";
        var salt = "salt";
        var title = "Senior Developer";
        var bio = "Experienced in .NET development.";

        // Act
        var freelancerUser = new FreelancerUser(email, fullName, passwordHash, salt, title, bio);

        Assert.Multiple(() =>
        {
            Assert.That(freelancerUser.Email, Is.EqualTo(email));
            Assert.That(freelancerUser.FullName, Is.EqualTo(fullName));
            Assert.That(freelancerUser.PasswordHash, Is.EqualTo(passwordHash));
            Assert.That(freelancerUser.Salt, Is.EqualTo(salt));
            Assert.That(freelancerUser.Role, Is.EqualTo(UserRole.Freelancer));
            Assert.That(freelancerUser.Title, Is.EqualTo(title));
            Assert.That(freelancerUser.Bio, Is.EqualTo(bio));
        });
    }

    [Test]
    public void UpdateTitle_ShouldChangeTitle()
    {
        // Arrange
        var freelancerUser = new FreelancerUser("freelancer@example.com", "Freelancer User", "hash", "salt", "Developer", "Bio");

        // Act
        var newTitle = "Lead Developer";
        freelancerUser.UpdateTitle(newTitle);

        // Assert
        Assert.That(freelancerUser.Title, Is.EqualTo(newTitle));
    }

    [Test]
    public void UpdateBio_ShouldChangeBio()
    {
        // Arrange
        var freelancerUser = new FreelancerUser("freelancer@example.com", "Freelancer User", "hash", "salt", "Developer", "Bio");

        // Act
        var newBio = "Updated bio information.";
        freelancerUser.UpdateBio(newBio);

        // Assert
        Assert.That(freelancerUser.Bio, Is.EqualTo(newBio));
    }

    [Test]
    public void Constructor_ShouldThrowArgumentException_WhenTitleIsNullOrWhiteSpace()
    {
        // Arrange
        var email = "freelancer@example.com";
        var fullName = "Freelancer User";
        var passwordHash = "hash";
        var salt = "salt";
        var bio = "Bio";

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new FreelancerUser(email, fullName, passwordHash, salt, "", bio));
        Assert.Throws<ArgumentException>(() =>
            new FreelancerUser(email, fullName, passwordHash, salt, "   ", bio));
    }

    [Test]
    public void Constructor_ShouldThrowArgumentException_WhenBioIsNullOrWhiteSpace()
    {
        // Arrange
        var email = "freelancer@example.com";
        var fullName = "Freelancer User";
        var passwordHash = "hash";
        var salt = "salt";
        var title = "Developer";

        // Act & Assert
        Assert.Throws<ArgumentException>(() =>
            new FreelancerUser(email, fullName, passwordHash, salt, title, ""));
        Assert.Throws<ArgumentException>(() =>
            new FreelancerUser(email, fullName, passwordHash, salt, title, "   "));
    }

    [Test]
    public void UpdateTitle_ShouldThrowArgumentException_WhenTitleIsNullOrWhiteSpace()
    {
        // Arrange
        var freelancerUser = new FreelancerUser("freelancer@example.com", "Freelancer User", "hash", "salt", "Developer", "Bio");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => freelancerUser.UpdateTitle(""));
        Assert.Throws<ArgumentException>(() => freelancerUser.UpdateTitle("   "));
    }

    [Test]
    public void UpdateBio_ShouldThrowArgumentException_WhenBioIsNullOrWhiteSpace()
    {
        // Arrange
        var freelancerUser = new FreelancerUser("freelancer@example.com", "Freelancer User", "hash", "salt", "Developer", "Bio");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => freelancerUser.UpdateBio(""));
        Assert.Throws<ArgumentException>(() => freelancerUser.UpdateBio("   "));
    }
}
