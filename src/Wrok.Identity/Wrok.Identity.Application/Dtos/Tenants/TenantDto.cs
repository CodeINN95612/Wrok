using Wrok.Identity.Domain.Entities;

namespace Wrok.Identity.Application.Dtos.Tenants;

public sealed record TenantDto(Guid Id, string Name);