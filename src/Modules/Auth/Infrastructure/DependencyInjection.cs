using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence;
using Hello100Admin.Modules.Auth.Application.Interfaces;
using Hello100Admin.Modules.Auth.Application.Services;
using Hello100Admin.Modules.Auth.Domain.Interfaces;
using Hello100Admin.Modules.Auth.Infrastructure.Persistence;
using Hello100Admin.Modules.Auth.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is not configured");
            
        services.AddScoped<IDbConnectionFactory>(provider => new DbConnectionFactory(connectionString));
        services.AddScoped<IUserRepository>(provider =>
            new UserRepository(
                provider.GetRequiredService<IDbConnectionFactory>(),
                provider.GetRequiredService<ILogger<UserRepository>>()
            )
        );
        services.AddScoped<IRefreshTokenRepository>(provider =>
            new RefreshTokenRepository(
                provider.GetRequiredService<IDbConnectionFactory>().CreateConnection(),
                provider.GetRequiredService<ILogger<RefreshTokenRepository>>()
            )
        );
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<ITokenService, JwtTokenService>();

        return services;
    }
}

