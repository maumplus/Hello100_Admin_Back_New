using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence;
using Hello100Admin.Modules.Admin.Infrastructure.Persistence;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories.AdminUser;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories.Member;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.AdminUser;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Member;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security.Hash;

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
        services.AddScoped<IAdminUserStore, AdminUserStore>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IMemberStore, MemberStore>();
        services.AddSingleton<IHasher, Sha256Hasher>();
        
        return services;
    }
}
