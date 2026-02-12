using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2016.Excel;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Common.Definitions.Enums;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;
using Hello100Admin.Modules.Admin.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Queries
{
    public class GetDoctorUntactQuery : IRequest<Result<GetDoctorUntactResult?>>
    {
        public string HospNo { get; set; } = string.Empty;
        public string EmplNo { get; set; } = string.Empty;
    }

    public class GetDoctorUntactQueryHandler : IRequestHandler<GetDoctorUntactQuery, Result<GetDoctorUntactResult?>>
    {
        private readonly string _adminImageUrl;
        private readonly IHospitalManagementStore _hospitalStore;
        private readonly ILogger<GetDoctorUntactQueryHandler> _logger;

        public GetDoctorUntactQueryHandler(IConfiguration config, IHospitalManagementStore hospitalStore, ILogger<GetDoctorUntactQueryHandler> logger)
        {
            _adminImageUrl = config["AdminImageUrl"] ?? string.Empty;
            _hospitalStore = hospitalStore;
            _logger = logger;
        }

        public async Task<Result<GetDoctorUntactResult?>> Handle(GetDoctorUntactQuery query, CancellationToken cancellationToken)
        {
            var tbEghisDoctUntanctEntity = await _hospitalStore.GetDoctorUntanctInfo(query.HospNo, query.EmplNo, cancellationToken);

            GetDoctorUntactResult? result = null;

            if (tbEghisDoctUntanctEntity != null)
            {
                var doctHistoryInfoList = tbEghisDoctUntanctEntity.DoctHistory.FromJson<List<TbEghisDoctUntanctEntity.DoctHistoryInfo>>();

                result = new GetDoctorUntactResult()
                {
                    DoctNm = tbEghisDoctUntanctEntity.DoctNm,
                    EmplNo = tbEghisDoctUntanctEntity.EmplNo,
                    DoctIntro = tbEghisDoctUntanctEntity.DoctIntro,
                    ClinicGuide = tbEghisDoctUntanctEntity.ClinicGuide,
                    DoctHistoryList = doctHistoryInfoList == null || doctHistoryInfoList.Count == 0 ? new List<string>() : doctHistoryInfoList.Select(x => x.history).ToList()
                };
            }

            return Result.Success<GetDoctorUntactResult?>(result);
        }
    }
}
