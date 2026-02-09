using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.Modules.Admin.Application.Features.Account.Responses;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Responses.Shared;
using MediatR;

namespace Hello100Admin.Modules.Admin.Application.Features.Account.Queries
{
    public class GetHospitalListQuery : IRequest<Result<PagedResult<GetHospitalResponse>>>
    {
        public AccountHospitalListSearchType SearchType { get; set; }
        public string Keyword { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }
}
