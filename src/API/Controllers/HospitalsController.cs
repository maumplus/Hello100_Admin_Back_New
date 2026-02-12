using Hello100Admin.API.Constracts.Admin.Hospitals;
using Hello100Admin.API.Extensions;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Exports;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.Hospitals.Commands;
using Hello100Admin.Modules.Admin.Application.Features.Hospitals.Queries;
using Hello100Admin.Modules.Admin.Application.Features.Hospitals.Results;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hello100Admin.API.Controllers
{
    /// <summary>
    /// 병원목록 API
    /// </summary>
    [Auth]
    [Route("api/hospitals")]
    public class HospitalsController : BaseController
    {
        #region FIELD AREA ****************************************
        private readonly ILogger<HospitalsController> _logger;
        private readonly IMediator _mediator;
        #endregion

        #region CONSTRUCTOR AREA ***********************************
        public HospitalsController(IMediator mediator, ILogger<HospitalsController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }
        #endregion

        #region ACTION METHOD AREA *******************************
        /// <summary>
        /// [전체 관리자] 병원목록 > 병원목록 > 조회
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<ListResult<SearchHospitalsResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchHospitals([FromQuery] GetHospitalsRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/hospitals [{AId}]", Aid);

            var query = req.Adapt<SearchHospitalsQuery>();

            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체 관리자] 병원목록 > 병원목록 > 병원편집 > 조회
        /// </summary>
        [HttpPost("detail")]
        [ProducesResponseType(typeof(ApiResponse<GetHospitalDetailResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHospitalDetail(GetHospitalDetailRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/hospitals/detail [{AId}]", Aid);

            var result = await _mediator.Send(new GetHospitalDetailQuery(req.HospKey), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체 관리자] 병원목록 > 병원목록 > 신규등록
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateHospital(CreateHospitalRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/hospitals [{AId}]", Aid);

            var command = req.Adapt<CreateHospitalCommand>();

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체 관리자] 병원목록 > 병원목록 > 병원편집 > 저장
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpPut]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateHospital(UpdateHospitalRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PUT /api/hospitals [{AId}]", Aid);

            var command = req.Adapt<UpdateHospitalCommand>();

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체 관리자] 병원목록 > 병원목록 > 병원편집 > 삭제
        /// </summary>
        /// <param name="req"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        [HttpDelete]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteHospital(DeleteHospitalRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("DELETE /api/hospitals [{AId}]", Aid);

            var result = await _mediator.Send(new DeleteHospitalCommand(req.HospKey), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체 관리자] 병원목록 > 병원목록 > 엑셀출력
        /// </summary>
        [HttpGet("export/excel")]
        [ProducesResponseType(typeof(ExcelFile), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportHospitalsExcel(int searchType, string? searchKeyword, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/hospitals/export/excel [{Aid}]", Aid);

            var result = await _mediator.Send(new ExportHospitalsExcelQuery(searchType, searchKeyword), cancellationToken);

            if (result.ErrorInfo != null)
                return result.ToActionResult(this);

            var file = result?.Data!;
            return File(file.Content, file.ContentType, file.FileName);
        }
        #endregion
    }
}
