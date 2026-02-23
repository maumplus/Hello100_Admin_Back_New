using System.Text.Json;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.AdminUser;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Results;
using Mapster;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.AdminUser.Queries
{
    public record GetHospitalAdminDetailQuery(string AId, string? IpAddress) : IQuery<Result<GetHospitalAdminDetailResult>>;

    public class GetHospitalAdminDetailQueryHandler : IRequestHandler<GetHospitalAdminDetailQuery, Result<GetHospitalAdminDetailResult>>
    {
        private readonly string _qrCodeApiUrl;
        private readonly string _qrTargetUrl;
        private readonly string _companyIps;
        private readonly string[] _companyIpArr;
        private readonly ILogger<GetHospitalAdminDetailQueryHandler> _logger;
        private readonly IAdminUserStore _adminUserStore;
        private readonly IDbSessionRunner _db;

        public GetHospitalAdminDetailQueryHandler(
            IConfiguration config,
            ILogger<GetHospitalAdminDetailQueryHandler> logger,
            IAdminUserStore adminUserStore,
            IDbSessionRunner db)
        {
            _companyIps = config["CompanyIps"] ?? string.Empty;
            _qrCodeApiUrl = config["QrCodeApiUrl"] ?? string.Empty;
            _qrTargetUrl = config["QrTargetUrl"] ?? string.Empty;
            _companyIpArr = string.IsNullOrEmpty(_companyIps) ? [] : _companyIps.Split(',');
            _logger = logger;
            _adminUserStore = adminUserStore;
            _db = db;
        }

        public async Task<Result<GetHospitalAdminDetailResult>> Handle(GetHospitalAdminDetailQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetHospitalAdminDetailQueryHandler");

            var qrInfo = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _adminUserStore.GetHospitalAdminDetailWithQrInfoAsync(session, req.AId, token), 
            ct);

            if (qrInfo == null)
                return Result.Success<GetHospitalAdminDetailResult>().WithError(AdminErrorCode.NotFoundHospital.ToError());

            var qrReq = new QrReqJson();
            qrReq.QrCodeText = $"{_qrTargetUrl}{qrInfo.Qid}";
            qrReq.FrameText = qrInfo.HospitalName;
            
            var queryDic = ToQueryDictionary(qrReq);

            var response = qrInfo.Adapt<GetHospitalAdminDetailResult>();
            response.AId = qrInfo.Aid;
            response.HospName = qrInfo.HospitalName;
            response.QrUrl = BuildQueryString(_qrCodeApiUrl, queryDic);
            response.IsCompanyIp = CheckIpAddress(req.IpAddress);

            return Result.Success(new GetHospitalAdminDetailResult());
        }

        private IDictionary<string, string> ToQueryDictionary<T>(T obj)
        {
            var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });

            var rawDict = JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;

            return rawDict
                .Where(x => x.Value != null && !string.IsNullOrWhiteSpace(x.Value.ToString()))
                .ToDictionary(
                    x => x.Key,
                    x => x.Value!.ToString()!
                );
        }

        private string BuildQueryString(
            string baseUrl,
            IDictionary<string, string> parameters)
        {
            var query = string.Join("&",
                parameters.Select(p =>
                    $"{Uri.EscapeDataString(p.Key)}={Uri.EscapeDataString(p.Value)}"));

            return baseUrl.Contains("?")
                ? $"{baseUrl}&{query}"
                : $"{baseUrl}?{query}";
        }

        private bool CheckIpAddress(string? ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress) == true)
                return false;

            return _companyIpArr.Contains(ipAddress);
        }
    }
}