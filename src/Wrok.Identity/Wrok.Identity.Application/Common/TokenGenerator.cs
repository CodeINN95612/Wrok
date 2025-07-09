using System.Security.Cryptography;

using Wrok.Identity.Application.Abstractions.Common;

namespace Wrok.Identity.Application.Common;
internal sealed class TokenGenerator : ITokenGenerator
{
    public string Generate(int byteCount)
    {
        var bytes = RandomNumberGenerator.GetBytes(byteCount);
        return Convert.ToBase64String(bytes);
    }
}
