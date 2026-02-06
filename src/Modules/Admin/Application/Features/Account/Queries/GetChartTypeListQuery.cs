using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.Account.Queries
{
    public class GetChartTypeListQuery : IRequest<Result<List<TbCommonEntity>>>
    {

    }
}
