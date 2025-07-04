﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Abstractions.UnitOfWork;
using Wrok.Identity.Infrastructure.Data;
using Wrok.Identity.Infrastructure.Repositories;

namespace Wrok.Identity.Infrastructure.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddWrokDb(configuration);

        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    private static IServiceCollection AddWrokDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WrokIdentityDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("wrok-identity-db")));

        //Apply migrations if needed
        using (var serviceProvider = services.BuildServiceProvider())
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WrokIdentityDbContext>();
            dbContext.Database.Migrate();
        }

        return services;
    }
}
