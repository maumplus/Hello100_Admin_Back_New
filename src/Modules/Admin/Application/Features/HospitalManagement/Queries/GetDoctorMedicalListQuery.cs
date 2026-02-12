using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Queries
{
    public class GetDoctorMedicalListQuery : IRequest<Result<GetDoctorMedicalListResult>>
    {
        public string HospNo { get; set; }
        public string EmplNo { get; set; }
    }

    public class GetDoctorMedicalListQueryHandler : IRequestHandler<GetDoctorMedicalListQuery, Result<GetDoctorMedicalListResult>>
    {
        private readonly IHospitalManagementStore _hospitalStore;
        private readonly ILogger<GetDoctorMedicalListQueryHandler> _logger;

        public GetDoctorMedicalListQueryHandler(
        IHospitalManagementStore hospitalStore,
        ILogger<GetDoctorMedicalListQueryHandler> logger)
        {
            _hospitalStore = hospitalStore;
            _logger = logger;
        }

        public async Task<Result<GetDoctorMedicalListResult>> Handle(GetDoctorMedicalListQuery query, CancellationToken cancellationToken)
        {
            var doctorMedicalInfoList = new List<DoctorMedicalInfo>();
            var deptCodeList = await _hospitalStore.GetHospitalMedicalListAsync(query.HospNo, cancellationToken);
            var mdCdList = await _hospitalStore.GetEghisDoctInfoMd(query.HospNo, query.EmplNo);

            var mdCds = mdCdList.Select(x => x.MdCd).ToList();

            foreach (var deptCode in deptCodeList)
            {
                var doctorMedicalInfo = new DoctorMedicalInfo()
                {
                    MdCd = deptCode.MdCd,
                    HospKey = deptCode.HospKey,
                    MdNm = deptCode.MdNm,
                    RegDt = deptCode.RegDt,
                    CheckYn = mdCds.Contains(deptCode.MdCd) ? "Y" : "N"
                };

                doctorMedicalInfoList.Add(doctorMedicalInfo);
            }

            var result = new GetDoctorMedicalListResult()
            {
                DoctorMedicalInfoList = doctorMedicalInfoList
            };

            return Result.Success(result);
        }
    }
}
