namespace Wrok.Identity.Application.Abstractions.Common;
public interface ITokenGenerator
{
    public const int DefaultByteCount = 32;
    public string Generate(int byteCount = DefaultByteCount);
}
