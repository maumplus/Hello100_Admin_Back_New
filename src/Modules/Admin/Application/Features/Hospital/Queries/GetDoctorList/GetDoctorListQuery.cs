using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Features.Hospital.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.Hospital.Queries.GetDoctorList
{
    /// <summary>
    /// 의료진 목록 조회 쿼리
    /// </summary>
    public class GetDoctorListQuery : IRequest<Result<List<GetDoctorListResponse>>>
    {
        public string HospNo { get; set; } = string.Empty;
    }
}
