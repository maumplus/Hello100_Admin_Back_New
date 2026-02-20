using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Domain.Entities;
using MediatR;

namespace Hello100Admin.Modules.Admin.Application.Features.Account.Queries
{
    public class GetChartTypeListQuery : IRequest<Result<List<TbCommonEntity>>>
    {

    }
}
