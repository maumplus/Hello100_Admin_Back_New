using Microsoft.Extensions.Configuration;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence;
using Hello100Admin.Modules.Admin.Infrastructure.Persistence;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories.AdminUser;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories.Member;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.AdminUser;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Member;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security.Hash;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ServiceUsage;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories.ServiceUsage;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Exports;
using Hello100Admin.Modules.Admin.Infrastructure.Exports.Excel;
using Hello100Admin.Modules.Admin.Infrastructure.External.Web.KakaoBiz;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.External;
using Microsoft.Extensions.DependencyInjection;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Common;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories.Common;
using Hello100Admin.Modules.Admin.Infrastructure.External.Web.EghisHome;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories.Hospital;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;

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
        services.AddScoped<ICurrentHospitalProfileProvider, CurrentHospitalProfileProvider>();
        services.AddScoped<IAdminUserRepository, AdminUserRepository>();
        services.AddScoped<IAdminUserStore, AdminUserStore>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IMemberStore, MemberStore>();
        services.AddScoped<IServiceUsageStore, ServiceUsageStore>();
        services.AddScoped<IServiceUsageRepository, ServiceUsageRepository>();
        services.AddScoped<IExcelExporter, ClosedXmlExcelExporter>();
        services.AddScoped<IHospitalStore, HospitalStore>();
        services.AddSingleton<IHasher, Sha256Hasher>();

        var kakaoBizUrl = configuration.GetSection("KakaoBizUrl").Value;
        var eghisHomeUrl = configuration.GetSection("EghisHomeUrl").Value;

        services.AddHttpClient<IBizApiClientService, KakaoBizApiClientService>(client =>
        {
            client.BaseAddress = new Uri($"{kakaoBizUrl}ws/api/kakao/hello100/send/history");
            client.Timeout = TimeSpan.FromSeconds(10);
        });
        services.AddHttpClient<IEghisHomeApiClientService, EghisHomeApiClientService>(client =>
        {
            client.BaseAddress = new Uri($"{eghisHomeUrl}");
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        return services;
    }
}
