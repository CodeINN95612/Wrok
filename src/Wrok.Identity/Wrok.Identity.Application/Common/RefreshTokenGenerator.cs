using System.Security.Cryptography;
using System.Text;

using Wrok.Identity.Application.Abstractions.Common;

namespace Wrok.Identity.Application.Common;
internal sealed class RefreshTokenGenerator : IRefreshTokenGenerator
{
    public const int DefaultByteCount = 32;
    public string Generate()
    {
        var bytes = RandomNumberGenerator.GetBytes(DefaultByteCount);
        return Convert.ToBase64String(bytes);
    }
}
