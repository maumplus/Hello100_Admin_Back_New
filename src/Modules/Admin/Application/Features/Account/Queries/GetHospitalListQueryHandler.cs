using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Account;
using Hello100Admin.Modules.Admin.Application.Features.Account.Responses;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Responses.Shared;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Account.Queries
{
    public class GetHospitalListQueryHandler : IRequestHandler<GetHospitalListQuery, Result<PagedResult<GetHospitalResponse>>>
    {
        private readonly IAccountStore _accountStore;
        private readonly ILogger<GetHospitalListQueryHandler> _logger;

        public GetHospitalListQueryHandler(
        IAccountStore accountStore,
        ILogger<GetHospitalListQueryHandler> logger)
        {
            _accountStore = accountStore;
            _logger = logger;
        }

        public async Task<Result<PagedResult<GetHospitalResponse>>> Handle(GetHospitalListQuery query, CancellationToken cancellationToken)
        {
            (var hospitalList, var totalCount) = await _accountStore.GetHospitalList(query.SearchType, query.Keyword, query.PageNo, query.PageSize, cancellationToken);

            var dtos = hospitalList.Adapt<List<GetHospitalResponse>>();

            var result = new PagedResult<GetHospitalResponse>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = query.PageNo,
                PageSize = query.PageSize
            };

            return Result.Success(result);
        }
    }
}
