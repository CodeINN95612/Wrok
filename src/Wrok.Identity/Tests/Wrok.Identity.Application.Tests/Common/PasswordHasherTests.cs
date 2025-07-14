using Wrok.Identity.Application.Common;

namespace Wrok.Identity.Application.Tests.Common;
[TestFixture]
internal class PasswordHasherTests
{
    [Test]
    public void Hash_ShouldReturnDifferentHashAndSalt_ForDifferentPasswords()
    {
        var hasher = new PasswordHasher();

        var (hash1, salt1) = hasher.Hash("password1");
        var (hash2, salt2) = hasher.Hash("password2");

        Assert.Multiple(() =>
        {
            Assert.That(hash1, Is.Not.EqualTo(hash2));
            Assert.That(salt1, Is.Not.EqualTo(salt2));
        });
    }

    [Test]
    public void Hash_ShouldReturnSameHash_ForSamePasswordAndSalt()
    {
        var hasher = new PasswordHasher();
        var password = "mySecretPassword";
        var (_, salt) = hasher.Hash(password);

        var hash1 = hasher.Hash(password, salt);
        var hash2 = hasher.Hash(password, salt);

        Assert.That(hash1, Is.EqualTo(hash2));
    }

    [Test]
    public void Hash_ShouldReturnDifferentHash_ForDifferentSalts()
    {
        var hasher = new PasswordHasher();
        var password = "mySecretPassword";
        var (_, salt1) = hasher.Hash(password);
        var (_, salt2) = hasher.Hash(password);

        var hash1 = hasher.Hash(password, salt1);
        var hash2 = hasher.Hash(password, salt2);

        Assert.That(hash1, Is.Not.EqualTo(hash2));
    }

    [Test]
    public void Hash_ShouldThrowFormatException_ForInvalidSalt()
    {
        var hasher = new PasswordHasher();
        var password = "password";
        var invalidSalt = "not_base64!";

        Assert.Throws<FormatException>(() => hasher.Hash(password, invalidSalt));
    }

    [TestCase("")]
    [TestCase(" ")]
    [TestCase("p@ssw0rd!")]
    [TestCase("123456")]
    public void Hash_ShouldReturnValidBase64_ForVariousPasswords(string password)
    {
        var hasher = new PasswordHasher();
        var (hash, salt) = hasher.Hash(password);

        Assert.Multiple(() =>
        {
            Assert.DoesNotThrow(() => Convert.FromBase64String(hash));
            Assert.DoesNotThrow(() => Convert.FromBase64String(salt));
        });
    }
}   
