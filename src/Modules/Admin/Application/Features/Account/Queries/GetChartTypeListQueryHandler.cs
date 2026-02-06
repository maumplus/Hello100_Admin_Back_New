using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Account;
using Hello100Admin.Modules.Admin.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hello100Admin.Modules.Admin.Application.Features.Account.Queries
{
    public class GetChartTypeListQueryHandler : IRequestHandler<GetChartTypeListQuery, Result<List<TbCommonEntity>>>
    {
        private readonly IAccountStore _accountStore;
        private readonly ILogger<GetChartTypeListQueryHandler> _logger;

        public GetChartTypeListQueryHandler(
        IAccountStore accountStore,
        ILogger<GetChartTypeListQueryHandler> logger)
        {
            _accountStore = accountStore;
            _logger = logger;
        }

        public async Task<Result<List<TbCommonEntity>>> Handle(GetChartTypeListQuery query, CancellationToken cancellationToken)
        {
            var result = await _accountStore.GetCommonList("14", cancellationToken);

            return Result.Success(result);
        }
    }
}
