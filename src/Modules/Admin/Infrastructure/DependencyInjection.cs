using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence;
using Hello100Admin.Modules.Admin.Domain.Interfaces;
using Hello100Admin.Modules.Admin.Infrastructure.Persistence;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories;

namespace Hello100Admin.Modules.Admin.Infrastructure;

/// <summary>
/// Admin 모듈 의존성 주입 설정
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddAdminInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection is not configured");

        services.AddScoped<IDbConnectionFactory>(provider => new DbConnectionFactory(connectionString));
        services.AddScoped<IAdminUserRepository, AdminUserRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        
        return services;
    }
}
