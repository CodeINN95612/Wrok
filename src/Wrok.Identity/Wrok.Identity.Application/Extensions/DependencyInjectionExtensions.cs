using FluentValidation;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Wrok.Identity.Application.Abstractions.Common;
using Wrok.Identity.Application.Common;
using Wrok.Identity.Application.Settings;

namespace Wrok.Identity.Application.Extensions;
public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddValidatorsFromAssembly(typeof(AssemblyMarker).Assembly, includeInternalTypes: true);
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(AssemblyMarker).Assembly);
            cfg.LicenseKey = configuration["Mediatr:License"];
        });

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IRegexValidator, RegexValidator>();
        services.AddScoped<IJwtGenerator, JwtGenerator>();
        services.AddScoped<ITokenGenerator, TokenGenerator>();

        services.AddSettings(configuration);

        return services;
    }

    private static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
        services.Configure<RefreshTokenSettings>(configuration.GetSection(RefreshTokenSettings.SectionName));
        return services;
    }
}
