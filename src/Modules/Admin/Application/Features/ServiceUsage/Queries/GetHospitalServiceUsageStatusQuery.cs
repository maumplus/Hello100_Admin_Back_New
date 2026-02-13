using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.ServiceUsage;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries
{
    public record GetHospitalServiceUsageStatusQuery : IQuery<Result<GetHospitalServiceUsageStatusResult>>
    {
        /// <summary>
        /// 페이지 번호
        /// </summary>
        public int PageNo { get; init; }
        /// <summary>
        /// 페이지 사이즈
        /// </summary>
        public int PageSize { get; init; }
        /// <summary>
        /// 조회 시작일 (yyyy-mm-dd)
        /// </summary>
        public string FromDate { get; init; } = default!;
        /// <summary>
        /// 조회 종료일 (yyyy-mm-dd)
        /// </summary>
        public string ToDate { get; init; } = default!;
        /// <summary>
        /// 검색 타입 [1: 병원명, 2: 요양기관번호]
        /// </summary>
        public int SearchType { get; init; }
        /// <summary>
        /// 검색 키워드
        /// </summary>
        public string? SearchKeyword { get; init; }
        /// <summary>
        /// QR 접수 체크 여부
        /// </summary>
        public string QrCheckInYn { get; init; } = default!;
        /// <summary>
        /// 오늘 접수 체크 여부
        /// </summary>
        public string TodayRegistrationYn { get; init; } = default!;
        /// <summary>
        /// 진료 예약 체크 여부
        /// </summary>
        public string AppointmentYn { get; init; } = default!;
        /// <summary>
        /// 비대면 진료 체크 여부
        /// </summary>
        public string TelemedicineYn { get; init; } = default!;
        /// <summary>
        /// 테스트병원 제외 여부
        /// </summary>
        public string ExcludeTestHospitalsYn { get; init; } = default!;
    }

    public class GetHospitalServiceUsageStatusQueryValidator : AbstractValidator<GetHospitalServiceUsageStatusQuery>
    {
        public GetHospitalServiceUsageStatusQueryValidator()
        {
            RuleFor(x => x.PageNo)
                .NotNull().GreaterThan(0).WithMessage("페이지 번호는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.PageSize)
                .NotNull().GreaterThan(0).WithMessage("페이지 사이즈는 필수이며 0보다 커야 합니다.");
        }
    }

    public class GetHospitalServiceUsageStatusQueryHandler : IRequestHandler<GetHospitalServiceUsageStatusQuery, Result<GetHospitalServiceUsageStatusResult>>
    {
        private readonly ILogger<GetHospitalServiceUsageStatusQueryHandler> _logger;
        private readonly IServiceUsageStore _serviceUsageStore;
        private readonly IDbSessionRunner _db;

        public GetHospitalServiceUsageStatusQueryHandler(
            ILogger<GetHospitalServiceUsageStatusQueryHandler> logger, 
            IServiceUsageStore serviceUsageStore, 
            IDbSessionRunner db)
        {
            _logger = logger;
            _serviceUsageStore = serviceUsageStore;
            _db = db;
        }

        public async Task<Result<GetHospitalServiceUsageStatusResult>> Handle(GetHospitalServiceUsageStatusQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handle GetHospitalServiceUsageStatusQueryHandler");

            var statusByServiceUnit = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _serviceUsageStore.GetServiceUnitReceptionStatusAsync(
                    session, req.FromDate, req.ToDate, req.SearchType, req.SearchKeyword, req.QrCheckInYn, 
                    req.TodayRegistrationYn, req.AppointmentYn, req.TelemedicineYn, req.ExcludeTestHospitalsYn, token),
            ct);

            var statusByHospitalUnit = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _serviceUsageStore.GetHospitalUnitReceptionStatusAsync(
                    session, req.PageNo, req.PageSize, req.FromDate, req.ToDate, req.SearchType, req.SearchKeyword, req.QrCheckInYn, 
                    req.TodayRegistrationYn, req.AppointmentYn, req.TelemedicineYn, req.ExcludeTestHospitalsYn, token),
            ct);

            var startRowNum = statusByHospitalUnit.TotalCount - ((req.PageNo - 1) * req.PageSize);

            if (statusByHospitalUnit.Items is not null)
            {
                for (int i = 0; i < statusByHospitalUnit.Items.Count; i++)
                {
                    statusByHospitalUnit.Items[i].RowNum = startRowNum - i;
                }
            }

            var result = new GetHospitalServiceUsageStatusResult()
            {
                StatusByServiceUnit = statusByServiceUnit,
                StatusByHospitalUnit = statusByHospitalUnit
            };

            return Result.Success(result);
        }
    }
}
