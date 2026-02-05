using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Responses.Shared;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Queries
{
    public class GetHospitalListQuery : IRequest<Result<PagedResult<GetHospitalResult>>>
    {
        public string ChartType { get; set; }
        public HospitalListSearchType SearchType { get; set; }
        public string Keyword { get; set; }
        public int PageNo { get; set; }
        public int PageSize { get; set; }
    }

    public class GetHospitalListQueryHandler : IRequestHandler<GetHospitalListQuery, Result<PagedResult<GetHospitalResult>>>
    {
        private readonly IHospitalStore _hospitalStore;
        private readonly ILogger<GetHospitalListQueryHandler> _logger;

        public GetHospitalListQueryHandler(
        IHospitalStore hospitalStore,
        ILogger<GetHospitalListQueryHandler> logger)
        {
            _hospitalStore = hospitalStore;
            _logger = logger;
        }

        public async Task<Result<PagedResult<GetHospitalResult>>> Handle(GetHospitalListQuery query, CancellationToken cancellationToken)
        {
            (var hospitalList, var totalCount) = await _hospitalStore.GetHospitalList(query.ChartType, query.SearchType, query.Keyword, query.PageNo, query.PageSize, cancellationToken);

            foreach (var hospital in hospitalList)
            {
                hospital.ClinicTimes = await _hospitalStore.GetHospMedicalTimeList(hospital.HospKey, cancellationToken);
                hospital.DeptCodes = await _hospitalStore.GetHospitalMedicalList(hospital.HospKey, cancellationToken);
                hospital.Keywords = await _hospitalStore.GetHospKeywordList(hospital.HospKey, cancellationToken);
                hospital.Images = await _hospitalStore.GetImageList(hospital.HospKey, cancellationToken);
                hospital.ClinicTimesNew = await _hospitalStore.GetHospMedicalTimeNewList(hospital.HospKey, cancellationToken);
                hospital.KeywordMasters = await _hospitalStore.GetKeywordMasterList(hospital.HospKey, cancellationToken);
            }

            var dtos = hospitalList.Adapt<List<GetHospitalResult>>();

            var result = new PagedResult<GetHospitalResult>
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
