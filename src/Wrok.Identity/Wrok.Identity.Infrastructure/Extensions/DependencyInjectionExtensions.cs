using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Wrok.Identity.Application.Abstractions.Providers;
using Wrok.Identity.Application.Abstractions.Repositories;
using Wrok.Identity.Application.Abstractions.UnitOfWork;
using Wrok.Identity.Application.Settings;
using Wrok.Identity.Infrastructure.Data;
using Wrok.Identity.Infrastructure.Providers;
using Wrok.Identity.Infrastructure.Repositories;

namespace Wrok.Identity.Infrastructure.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuth(configuration);

        services.AddWrokDb(configuration);

        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IInvitationRepository, InvitationRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<IIdentityProvider, IdentityProvider>();

        return services;
    }

    private static IServiceCollection AddWrokDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WrokIdentityDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("wrok-identity-db")));

        //Apply migrations if needed
        if (!EF.IsDesignTime)
        {
            using var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<WrokIdentityDbContext>();
            dbContext.Database.Migrate();
        }

        return services;
    }

    private static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var settings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>();
        if (settings is null)
        {
            throw new ArgumentNullException(nameof(settings), "JWT settings are not configured.");
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = settings.Issuer,
                    ValidAudience = settings.Audience,
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(settings.Secret))
                };
            });
        services.AddAuthorization();

        return services;
    }

    public static IApplicationBuilder UseInfrastructureAuth(this IApplicationBuilder app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        // Middleware to set the identity provider's UserId and TenantId from the authenticated user
        app.Use(async (context, next) =>
        {
            var identityProvider = context.RequestServices.GetRequiredService<IIdentityProvider>();

            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var tenantId = context.User.FindFirst("tenant")?.Value;

                if (!string.IsNullOrEmpty(userId) && Guid.TryParse(userId, out Guid userIdGuid))
                {
                    identityProvider.UserId = new(userIdGuid);
                }

                if (!string.IsNullOrEmpty(tenantId) && Guid.TryParse(tenantId, out Guid tenantIdGuid))
                {
                    identityProvider.TenantId = new(tenantIdGuid);
                }
            }

            await next();
        });

        return app;
    }

}
