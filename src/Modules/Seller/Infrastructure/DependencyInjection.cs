using System.Net.Http.Headers;
using System.Net.Mime;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.Persistence.Bank;
using Hello100Admin.Modules.Seller.Application.Common.Abstractions.Persistence.Seller;
using Hello100Admin.Modules.Seller.Infrastructure.Configuration.Options;
using Hello100Admin.Modules.Seller.Infrastructure.External.Http;
using Hello100Admin.Modules.Seller.Infrastructure.External.Web.Seller;
using Hello100Admin.Modules.Seller.Infrastructure.Persistence;
using Hello100Admin.Modules.Seller.Infrastructure.Repositories.Bank;
using Hello100Admin.Modules.Seller.Infrastructure.Repositories.Seller;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Hello100Admin.Modules.Seller.Infrastructure
{
    /// <summary>
    /// Seller 모듈 의존성 주입 설정
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddSellerInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection is not configured");

            services.Configure<KcpRemitOptions>(configuration.GetSection("KcpRemitOptions"));

            services.AddScoped<IDbConnectionFactory>(provider => new DbConnectionFactory(connectionString));
            services.AddScoped<ISellerRepository, SellerRepository>();
            services.AddScoped<ISellerStore, SellerStore>();
            services.AddScoped<IBankStore, BankStore>();
            services.AddScoped<IKcpRemitService, KcpRemitService>();

            services.AddHttpClient<IWebRequestService, WebRequestService>(client =>
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(MediaTypeNames.Application.Json));
            });

            return services;
        }
    }
}
