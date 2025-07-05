using Microsoft.Extensions.DependencyInjection;

using Wrok.Identity.Application.Abstractions.Common;
using Wrok.Identity.Application.Abstractions.Services;
using Wrok.Identity.Application.Common;
using Wrok.Identity.Application.Services;

namespace Wrok.Identity.Application.Extensions;
public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IRegexValidator, RegexValidator>();

        return services;
    }
}
