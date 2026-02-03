using Microsoft.Extensions.Configuration;
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
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.VisitPurpose;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories.VisitPurpose;
using Hello100Admin.Modules.Admin.Infrastructure.Persistence.MySql;
using Hello100Admin.Modules.Admin.Infrastructure.Configuration.Options;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories.Hospital;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ApprovalRequest;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories.ApprovalRequest;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories.HospitalStatistics;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories.HospitalUser;
using Hello100Admin.Modules.Admin.Domain.Repositories;

namespace Hello100Admin.Modules.Admin.Infrastructure;

/// <summary>
/// Admin 모듈 의존성 주입 설정
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddAdminInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DbConnectionOptions>(configuration.GetSection("ConnectionStrings:DefaultConnection"));

        services.AddScoped<IDbSessionRunner, DbSessionRunner>();
        services.AddScoped<IDbConnectionFactory, MySqlConnectionFactory>();
        services.AddScoped<ICurrentHospitalProfileProvider, CurrentHospitalProfileProvider>();
        services.AddScoped<IAdminUserRepository, AdminUserRepository>();
        services.AddScoped<IAdminUserStore, AdminUserStore>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<IMemberStore, MemberStore>();
        services.AddScoped<IServiceUsageStore, ServiceUsageStore>();
        services.AddScoped<IServiceUsageRepository, ServiceUsageRepository>();
        services.AddScoped<IVisitPurposeStore, VisitPurposeStore>();
        services.AddScoped<IVisitPurposeRepository, VisitPurposeRepository>();
        services.AddScoped<IApprovalRequestStore, ApprovalRequestStore>();
        services.AddScoped<IHospitalStatisticsStore, HospitalStatisticsStore>();
        services.AddScoped<IHospitalUserStore, HospitalUserStore>();
        services.AddScoped<IHospitalUserRepository, HospitalUserRepository>();
        services.AddScoped<IExcelExporter, ClosedXmlExcelExporter>();
        services.AddScoped<IHospitalStore, HospitalStore>();
        services.AddSingleton<IHasher, Sha256Hasher>();

        var kakaoBizUrl = configuration.GetSection("KakaoBizUrl").Value;
        var eghisHomeUrl = configuration.GetSection("EghisHomeUrl").Value;

        services.AddHttpClient<IBizApiClientService, KakaoBizApiClientService>(client =>
        {
            client.BaseAddress = new Uri($"{kakaoBizUrl}");
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
