namespace Wrok.Identity.Application.Abstractions.Common;
public interface IPasswordHasher
{
    public (string passwordHash, string salt) Hash(string password);
    public string Hash(string password, string salt);
}
