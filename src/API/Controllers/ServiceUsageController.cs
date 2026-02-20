using Hello100Admin.API.Constracts.Admin.ServiceUsage;
using Hello100Admin.API.Extensions;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Exports;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Commands.SubmitAlimtalkApplication;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.ExportExaminationResultAlimtalkHistoriesExcel;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.ExportUntactMedicalHistoriesExcel;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.GetExaminationResultAlimtalkApplicationInfo;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.GetRegistrationAlimtalkApplicationInfo;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.GetUntactMedicalPaymentDetail;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.SearchExaminationResultAlimtalkHistories;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.SearchUntactMedicalHistories;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Responses.GetExaminationResultAlimtalkApplicationInfo;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Responses.GetRegistrationAlimtalkApplicationInfo;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Responses.GetUntactMedicalPaymentDetail;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Responses.SearchExaminationResultAlimtalkHistories;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Responses.SearchUntactMedicalHistories;
using Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Results;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hello100Admin.API.Controllers
{
    /// <summary>
    /// 서비스 이용 관리 API Controller
    /// </summary>
    [Auth]
    [Route("api/service-usage")]
    public class ServiceUsageController : BaseController
    {
        private readonly ILogger<ServiceUsageController> _logger;
        private readonly IMediator _mediator;

        public ServiceUsageController(IMediator mediator, ILogger<ServiceUsageController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        #region 병원관리자 서비스 *******************************************
        /// <summary>
        /// 비대면 진료 내역
        /// </summary>
        [HttpPost("untact-medical/search")]
        [ProducesResponseType(typeof(ApiResponse<SearchUntactMedicalHistoriesResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchUntactMedicalHistories(SearchUntactMedicalHistorysRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/service-usage/untact-medical/search [{Aid}]", Aid);

            var query = req.Adapt<SearchUntactMedicalHistoriesQuery>() with { HospNo = base.HospNo };
            
            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 비대면 진료 결제 내역 상세 조회
        /// </summary>
        [HttpGet("untact-medical/payments/{paymentId}")]
        [ProducesResponseType(typeof(ApiResponse<GetUntactMedicalPaymentDetailResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUntactMedicalPaymentDetail(string paymentId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/service-usage/untact-medical/payments/{id} [{Aid}]", paymentId, Aid);
            
            var result = await _mediator.Send(new GetUntactMedicalPaymentDetailQuery(paymentId), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 비대면 진료 내역 Excel 출력
        /// </summary>
        [HttpPost("untact-medical/export/excel")]
        [ProducesResponseType(typeof(ExcelFile), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportUntactMedicalHistoriesExcel(ExportUntactMedicalHistoriesExcelRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/service-usage/untact-medical/export/excel [{Aid}]", Aid);
            
            var query = req.Adapt<ExportUntactMedicalHistoriesExcelQuery>() with { HospNo = base.HospNo };

            var result = await _mediator.Send(query, cancellationToken);

            if (result.ErrorInfo != null)
                return result.ToActionResult(this);

            var file = result?.Data!;
            return File(file.Content, file.ContentType, file.FileName);
        }

        /// <summary>
        /// 진단검사결과 알림톡 발송 내역
        /// </summary>
        [HttpPost("examination-results/alimtalk/histories/search")]
        [ProducesResponseType(typeof(ApiResponse<SearchExaminationResultAlimtalkHistoriesResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchExaminationResultAlimtalkHistories(SearchExaminationResultAlimtalkHistoriesRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/service-usage/examination-results/alimtalk/histories/search [{Aid}]", Aid);
            
            var query = req.Adapt<SearchExaminationResultAlimtalkHistoriesQuery>() with 
            {
                FromDate = req.DateRangeType == "0" ? DateTime.Now.ToString("yyyy-MM-dd") : req.FromDate,
                ToDate = req.DateRangeType == "0" ? DateTime.Now.ToString("yyyy-MM-dd") : req.ToDate,
                HospNo = base.HospNo
            };

            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 진단검사결과 알림톡 발송 내역 Excel 출력
        /// </summary>
        [HttpPost("examination-results/alimtalk/histories/export/excel")]
        [ProducesResponseType(typeof(ExcelFile), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportExaminationResultAlimtalkHistoriesExcel(ExportExaminationResultAlimtalkHistoriesExcelRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/service-usage/examination-results/alimtalk/histories/export/excel [{Aid}]", Aid);

            var query = req.Adapt<ExportExaminationResultAlimtalkHistoriesExcelQuery>() with
            {
                FromDate = req.DateRangeType == "0" ? DateTime.Now.ToString("yyyy-MM-dd") : req.FromDate,
                ToDate = req.DateRangeType == "0" ? DateTime.Now.ToString("yyyy-MM-dd") : req.ToDate,
                HospNo = base.HospNo
            };

            var result = await _mediator.Send(query, cancellationToken);

            if (result.ErrorInfo != null)
                return result.ToActionResult(this);

            var file = result?.Data!;
            return File(file.Content, file.ContentType, file.FileName);
        }

        /// <summary>
        /// 알림톡 발송 서비스 신청(진료접수) 조회
        /// </summary>
        [HttpGet("alimtalk-service/registration/application-info")]
        [ProducesResponseType(typeof(ApiResponse<GetRegistrationAlimtalkApplicationInfoResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRegistrationAlimtalkApplicationInfo(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/service-usage/alimtalk-service/registration/application-info [{Aid}]", Aid);

            var result = await _mediator.Send(new GetRegistrationAlimtalkApplicationInfoQuery(base.HospNo, base.HospKey), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 알림톡 발송 서비스 신청(검사결과) 조회
        /// </summary>
        [HttpGet("alimtalk-service/examination-results/application-info")]
        [ProducesResponseType(typeof(ApiResponse<GetExaminationResultAlimtalkApplicationInfoResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExaminationResultAlimtalkApplicationInfo(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/service-usage/alimtalk-service/examination-results/application-info [{Aid}]", Aid);
            
            var result = await _mediator.Send(new GetExaminationResultAlimtalkApplicationInfoQuery(base.HospNo, base.HospKey), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// 알림톡 발송 서비스 신청 [진료접수, 검사결과] > 신청
        /// </summary>
        [HttpPost("alimtalk-service/submit")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> SubmitAlimtalkApplication(SubmitAlimtalkApplicationRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/service-usage/alimtalk-service/submit [{Aid}]", Aid);

            var command = req.Adapt<SubmitAlimtalkApplicationCommand>() with { HospNo = base.HospNo, HospKey = base.HospKey };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }
        #endregion

        #region 전체관리자 서비스 *******************************************
        /// <summary>
        /// [전체 관리자] 서비스이용관리 > 병원별 서비스이용현황 > 조회
        /// </summary>
        [HttpGet("hospitals")]
        [ProducesResponseType(typeof(ApiResponse<GetHospitalServiceUsageStatusResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHospitalServiceUsageStatus([FromQuery] GetHospitalServiceUsageStatusRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/service-usage/hospitals [{Aid}]", Aid);

            var query = req.Adapt<GetHospitalServiceUsageStatusQuery>();

            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체 관리자] 서비스이용관리 > 병원별 서비스이용현황 > 엑셀출력(상단) -> 서비스 단위 접수현황
        /// </summary>
        [HttpGet("receptions/excel/by-service-unit")]
        [ProducesResponseType(typeof(ExcelFile), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportServiceUnitReceptionStatusExcel([FromQuery] ExportServiceUnitReceptionStatusExcelRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/service-usage/receptions/excel/by-service-unit [{Aid}]", Aid);

            var query = req.Adapt<ExportServiceUnitReceptionStatusExcelQuery>();

            var result = await _mediator.Send(query, cancellationToken);

            if (result.ErrorInfo != null)
                return result.ToActionResult(this);

            var file = result?.Data!;
            return File(file.Content, file.ContentType, file.FileName);
        }

        /// <summary>
        /// [전체 관리자] 서비스이용관리 > 병원별 서비스이용현황 > 엑셀출력(하단) -> 병원 단위 접수현황
        /// </summary>
        [HttpGet("receptions/excel/by-hospital-unit")]
        [ProducesResponseType(typeof(ExcelFile), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportHospitalUnitReceptionStatusExcel([FromQuery] ExportHospitalUnitReceptionStatusExcelRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/service-usage/receptions/excel/by-hospital-unit [{Aid}]", Aid);

            var query = req.Adapt<ExportHospitalUnitReceptionStatusExcelQuery>();

            var result = await _mediator.Send(query, cancellationToken);

            if (result.ErrorInfo != null)
                return result.ToActionResult(this);

            var file = result?.Data!;
            return File(file.Content, file.ContentType, file.FileName);
        }

        /// <summary>
        /// [전체 관리자] 서비스이용관리 > 병원별 서비스이용현황 > 엑셀출력(헬로100접수현황)
        /// </summary>
        [HttpGet("receptions/excel/by-hello100")]
        [ProducesResponseType(typeof(ExcelFile), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportHello100ReceptionStatusExcel(string fromDate, string toDate, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/service-usage/receptions/excel/by-hello100 [{Aid}]", Aid);

            var result = await _mediator.Send(new ExportHello100ReceptionStatusExcelQuery(fromDate, toDate), cancellationToken);

            if (result.ErrorInfo != null)
                return result.ToActionResult(this);

            var file = result?.Data!;
            return File(file.Content, file.ContentType, file.FileName);
        }

        /// <summary>
        /// [전체 관리자] 서비스이용관리 > 비대면진료 현황 > 조회
        /// </summary>
        [HttpGet("untact-medical/status")]
        [ProducesResponseType(typeof(ApiResponse<GetUntactMedicalUsageStatusResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUntactMedicalUsageStatus([FromQuery] GetUntactMedicalUsageStatusRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/service-usage/untact-medical/status [{Aid}]", Aid);

            var query = req.Adapt<GetUntactMedicalUsageStatusQuery>();

            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체 관리자] 서비스이용관리 > 비대면진료 현황 > 엑셀출력
        /// </summary>
        [HttpGet("untact-medical/status/excel")]
        [ProducesResponseType(typeof(ExcelFile), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportUntactMedicalUsageStatusExcel([FromQuery] ExportUntactMedicalUsageStatusExcelRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/service-usage/untact-medical/status/excel [{Aid}]", Aid);

            var query = req.Adapt<ExportUntactMedicalUsageStatusExcelQuery>();

            var result = await _mediator.Send(query, cancellationToken);

            if (result.ErrorInfo != null)
                return result.ToActionResult(this);

            var file = result?.Data!;
            return File(file.Content, file.ContentType, file.FileName);
        }
        #endregion
    }
}
