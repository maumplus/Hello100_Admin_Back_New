using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Responses.Shared;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;
using Mapster;
using MediatR;
using Microsoft.Extensions.Configuration;
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
        private readonly string _adminImageUrl;
        private readonly IHospitalManagementStore _hospitalStore;
        private readonly ILogger<GetHospitalListQueryHandler> _logger;

        public GetHospitalListQueryHandler(
            IConfiguration config,
            IHospitalManagementStore hospitalStore,
            ILogger<GetHospitalListQueryHandler> logger)
        {
            _adminImageUrl = config["AdminImageUrl"] ?? string.Empty;
            _hospitalStore = hospitalStore;
            _logger = logger;
        }

        public async Task<Result<PagedResult<GetHospitalResult>>> Handle(GetHospitalListQuery query, CancellationToken cancellationToken)
        {
            (var hospitalList, var totalCount) = await _hospitalStore.GetHospitalListAsync(query.ChartType, query.SearchType, query.Keyword, query.PageNo, query.PageSize, cancellationToken);

            foreach (var hospital in hospitalList)
            {
                hospital.ClinicTimes = await _hospitalStore.GetHospMedicalTimeListAsync(hospital.HospKey, cancellationToken);
                hospital.DeptCodes = await _hospitalStore.GetHospitalMedicalListAsync(hospital.HospKey, cancellationToken);
                hospital.Keywords = await _hospitalStore.GetHospKeywordListAsync(hospital.HospKey, cancellationToken);
                hospital.Images = await _hospitalStore.GetImageListAsync(hospital.HospKey, cancellationToken);
                hospital.ClinicTimesNew = await _hospitalStore.GetHospMedicalTimeNewListAsync(hospital.HospKey, cancellationToken);
                hospital.KeywordMasters = await _hospitalStore.GetKeywordMasterListAsync(hospital.HospKey, cancellationToken);

                if (hospital.Images != null && hospital.Images.Count > 0)
                {
                    foreach (var img in hospital.Images)
                    {
                        img.Url = $"{_adminImageUrl}{img.Url}";
                    }
                }
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
