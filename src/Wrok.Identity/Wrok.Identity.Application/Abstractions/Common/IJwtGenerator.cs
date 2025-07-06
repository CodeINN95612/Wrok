using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Application.Abstractions.Common;
public interface IJwtGenerator
{
    public string Generate(User user);
}
