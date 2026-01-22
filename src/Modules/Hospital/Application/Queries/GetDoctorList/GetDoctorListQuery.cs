using MediatR;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.HospitalAdmin.Application.DTOs;

namespace Hello100Admin.Modules.Hospital.Application.Queries.GetDoctorList
{
    /// <summary>
    /// 의료진 목록 조회 쿼리
    /// </summary>
    public class GetDoctorListQuery : IRequest<Result<List<DoctorDto>>>
    {
        public string HospNo { get; set; } = string.Empty;
    }
}
