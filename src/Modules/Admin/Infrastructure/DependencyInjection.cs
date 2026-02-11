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
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ApprovalRequest;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories.ApprovalRequest;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories.HospitalStatistics;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories.HospitalUser;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories.Advertisement;
using Hello100Admin.Modules.Admin.Infrastructure.External.Sftp;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories.HospitalManagement;
using Hello100Admin.Modules.Admin.Infrastructure.External.Web.BizSite;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Account;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories.Account;
using Hello100Admin.Modules.Admin.Infrastructure.Repositories.Notice;

namespace Hello100Admin.Modules.Admin.Infrastructure;

/// <summary>
/// Admin 모듈 의존성 주입 설정
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddAdminInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DbConnectionOptions>(configuration.GetSection("ConnectionStrings:DefaultConnection"));
        services.Configure<SftpOptions>(configuration.GetSection("SftpOptions"));

        services.AddScoped<IDbSessionRunner, DbSessionRunner>();
        services.AddScoped<IDbConnectionFactory, MySqlConnectionFactory>();
        services.AddScoped<ICurrentHospitalProfileProvider, CurrentHospitalProfileProvider>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IAccountStore, AccountStore>();
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
        services.AddScoped<IHospitalManagementStore, HospitalManagementStore>();
        services.AddScoped<IHospitalManagementRepository, HospitalManagementRepository>();
        services.AddScoped<IAdvertisementStore, AdvertisementStore>();
        services.AddScoped<IAdvertisementRepository, AdvertisementRepository>();
        services.AddScoped<INoticeStore, NoticeStore>();
        services.AddScoped<INoticeRepository, NoticeRepository>();
        services.AddScoped<ISftpClientService, SftpClientService>();
        services.AddScoped<IExcelExporter, ClosedXmlExcelExporter>();
        services.AddScoped<IRequestsManagementStore, IRequestsManagementStore>();
        services.AddSingleton<IHasher, Sha256Hasher>();

        var kakaoBizUrl = configuration.GetSection("KakaoBizUrl").Value;
        var eghisHomeUrl = configuration.GetSection("EghisHomeUrl").Value;
        var bizApiDefaultUrl = configuration.GetSection("BizApiDefaultUrl").Value;

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
        services.AddHttpClient<IBizSiteApiClientService, BizSiteApiClientService>(client =>
        {
            client.BaseAddress = new Uri($"{bizApiDefaultUrl}");
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        return services;
    }
}
