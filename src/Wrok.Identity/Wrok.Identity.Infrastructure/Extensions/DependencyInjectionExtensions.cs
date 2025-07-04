using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Abstractions.UnitOfWork;
using Wrok.Identity.Infrastructure.Data;
using Wrok.Identity.Infrastructure.Repositories;

namespace Wrok.Identity.Infrastructure.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WrokIdentityDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("work-identity-db")));

        services.AddScoped<ITenantRepository, TenantRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }
}
