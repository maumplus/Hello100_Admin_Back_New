using MediatR;
using Hello100Admin.API.Extensions;
using Microsoft.AspNetCore.Mvc;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Queries;
using Hello100Admin.API.Constracts.Admin.HospitalManagement;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Commands;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Mapster;
using Hello100Admin.API.Infrastructure.Attributes;

namespace Hello100Admin.API.Controllers
{
    /// <summary>
    /// 병원정보관리 API Controller
    /// </summary>
    [Auth]
    [Route("api/hospital-management")]
    public class HospitalManagementController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<HospitalManagementController> _logger;

        public HospitalManagementController(
        IMediator mediator,
        ILogger<HospitalManagementController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// [전체관리자] 병원정보관리 > 병원정보관리 > 서비스이용병원목록 조회
        /// </summary>
        [HttpGet("hospitals")]
        [ProducesResponseType(typeof(ApiResponse<GetHospitalResult>), StatusCodes.Status200OK)]
        [Obsolete("전체 관리자 기능은 미완성(조회 조건 없음)입니다.")]
        public async Task<IActionResult> GetHospitalList(CancellationToken cancellationToken = default)
        {
            /*var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
            {
                return Unauthorized();
            }*/

            _logger.LogInformation("GET /api/hospital-management/hospitals");

            var query = new GetHospitalListQuery()
            {
                ChartType = "%",
                SearchType = 0,
                Keyword = "",
                PageNo = 0,
                PageSize = 100
            };

            var result = await _mediator.Send(query, cancellationToken);

            // 중앙화된 매퍼로 Result -> IActionResult 변환
            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원관리자] 병원정보관리 > 병원정보관리 > 조회
        /// </summary>
        [HttpGet("hospital")]
        [ProducesResponseType(typeof(ApiResponse<GetHospitalResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHospital(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/hospital-management/hospital");

            var result = await _mediator.Send(new GetHospitalQuery(base.HospNo), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원관리자] 병원정보관리 > 병원정보관리 > 저장
        /// </summary>
        [HttpPost("hospital")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpsertHospital([FromForm] UpsertHospitalRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/hospital-management/hospital");

            var payloads = this.GetImagePayload(req.NewImages);

            var command = req.Adapt<UpsertHospitalCommand>() with
            {
                AId = base.Aid,
                HospNo = base.HospNo,
                HospKey = base.HospKey,
                NewImages = payloads
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        private List<FileUploadPayload>? GetImagePayload(List<IFormFile>? images)
        {
            if (images == null || images.Count <= 0)
                return null;

            var payloads = new List<FileUploadPayload>();

            foreach (var image in images)
            {
                var payload = new FileUploadPayload(image.FileName, image.ContentType, image.Length, () => image.OpenReadStream());
                payloads.Add(payload);
            }

            return payloads;
        }

        ///// <summary>
        ///// [병원정보관리 > Hello100 설정]Hello100 설정정보 API
        ///// </summary>
        //[HttpGet("hospital/setting")]
        //[ProducesResponseType(typeof(GetHospitalResult), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> GetHospitalSetting(CancellationToken cancellationToken = default)
        //{
        //    _logger.LogInformation("GET /api/hospital-management/hospital/setting");

        //    var result = await _mediator.Send(new GetHospitalSettingQuery(base.HospKey), cancellationToken);

        //    return result.ToActionResult(this);
        //}

        ///// <summary>
        ///// [병원정보관리 > 의료진관리]의료진 목록 API
        ///// </summary>
        //[HttpGet("doctors")]
        //[ProducesResponseType(typeof(List<GetDoctorListResult>), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<IActionResult> GetDoctorList(CancellationToken cancellationToken = default)
        //{
        //    /*var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        //    if (userId == null)
        //    {
        //        return Unauthorized();
        //    }*/

        //    _logger.LogInformation("GET /api/hospital-management/doctors");

        //    var query = new GetDoctorListQuery()
        //    {
        //        HospNo = "10350072"
        //    };
        //    var result = await _mediator.Send(query, cancellationToken);

        //    // 중앙화된 매퍼로 Result -> IActionResult 변환
        //    return result.ToActionResult(this);
        //}
    }
}
