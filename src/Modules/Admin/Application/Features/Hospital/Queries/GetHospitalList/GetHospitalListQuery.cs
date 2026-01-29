using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Responses.Shared;
using Hello100Admin.Modules.Admin.Application.Features.Hospital.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.Hospital.Queries.GetHospitalList
{
    public class GetHospitalListQuery : IRequest<Result<PagedResult<GetHospitalResponse>>>
    {
        public string ChartType { get; set; }
        public HospitalListSearchType SearchType { get; set; }
        public string Keyword { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
}
