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
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Encodings.Web;
using Hello100Admin.Modules.Admin.Domain.Entities;
using System.Collections.Generic;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Security;
using Hello100Admin.Modules.Admin.Application.Common.Definitions.Enums;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.External;
using Hello100Admin.Modules.Admin.Infrastructure.External.Sftp;

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
        private readonly ICryptoService _cryptoService;
        private readonly ISftpClientService _sftpClientService;
        private readonly ILogger<HospitalManagementController> _logger;

        public HospitalManagementController(
        IMediator mediator,
        ICryptoService cryptoService,
        ISftpClientService sftpClientService,
        ILogger<HospitalManagementController> logger)
        {
            _mediator = mediator;
            _cryptoService = cryptoService;
            _sftpClientService = sftpClientService;
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

        /// <summary>
        /// [병원관리자] 병원정보관리 > Hello100 설정 > 조회
        /// </summary>
        [HttpGet("hello100-setting")]
        [ProducesResponseType(typeof(ApiResponse<GetHello100SettingResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHello100Setting(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/hospital-management/hello100-setting");

            var result = await _mediator.Send(new GetHello100SettingQuery(base.HospNo, base.HospKey), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원관리자] 병원정보관리 > Hello100 설정 > 저장
        /// </summary>
        [HttpPost("hello100-setting")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpsertHello100Setting(UpsertHello100SettingRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/hospital-management/hello100-setting");

            var command = req.Adapt<UpsertHello100SettingCommand>() with
            {
                HospNo = base.HospNo,
                HospKey = base.HospKey,
                Role = this.SetRole(req.Roles)
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원관리자] 병원정보관리 > 헬로데스크 설정 > 조회
        /// </summary>
        [HttpGet("hello-desk-setting")]
        [ProducesResponseType(typeof(ApiResponse<GetDeviceSettingResult<TabletRo>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHelloDeskSetting(string? emplNo, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/hospital-management/hello-desk-setting");

            var result = await _mediator.Send(new GetHelloDeskSettingQuery(base.HospNo, base.HospKey, emplNo), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원관리자] 병원정보관리 > 키오스크 설정 > 조회
        /// </summary>
        [HttpGet("kiosk-setting")]
        [ProducesResponseType(typeof(ApiResponse<GetDeviceSettingResult<KioskRo>>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetKioskSetting(string? emplNo, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/hospital-management/kiosk-setting");

            var result = await _mediator.Send(new GetKioskSettingQuery(base.HospNo, base.HospKey, emplNo), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원관리자] 병원정보관리 > 헬로데스크, 키오스크 설정 > 저장
        /// </summary>
        [HttpPost("device-setting")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpsertDeviceSetting(UpsertDeviceSettingRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/hospital-management/device-setting");

            string? setJson = this.CustomJsonToString(req.SetJson);

            var command = req.Adapt<UpsertDeviceSettingCommand>() with
            {
                HospNo = base.HospNo,
                HospKey = base.HospKey,
                SetJson = setJson
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원정보관리 > 의료진관리]의료진 목록 API
        /// </summary>
        [HttpGet("doctors")]
        [ProducesResponseType(typeof(List<GetDoctorListResult>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDoctorList(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/hospital-management/doctors");

            var query = new GetDoctorListQuery()
            {
                HospNo = HospNo
            };
            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원정보관리 > 의료진관리]의료진 목록 수정 API
        /// </summary>
        [HttpPatch("doctors")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchDoctorList(PatchDoctorListRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PATCH /api/hospital-management/doctors");

            List<Modules.Admin.Application.Features.HospitalManagement.Commands.PatchDoctorInfo> doctorList = new List<Modules.Admin.Application.Features.HospitalManagement.Commands.PatchDoctorInfo>();

            foreach (var doctorInfo in request.DoctorList)
            {
                doctorList.Add(new Modules.Admin.Application.Features.HospitalManagement.Commands.PatchDoctorInfo()
                {
                    HospNo = base.HospNo,
                    EmplNo = doctorInfo.EmplNo,
                    FrontViewRole = doctorInfo.FrontViewRole
                });
            }

            var query = new PatchDoctorListCommand()
            {
                HospNo = base.HospNo,
                DoctorList = doctorList
            };
            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원정보관리 > 의료진관리]의료진 상세 API
        /// </summary>
        [HttpGet("doctor/{emplNo}")]
        [ProducesResponseType(typeof(GetDoctorResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDoctor(string emplNo, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/hospital-management/doctors");

            var query = new GetDoctorQuery()
            {
                HospNo = HospNo,
                EmplNo = emplNo
            };
            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원정보관리 > 의료진관리]의료진 수정 API
        /// </summary>
        [HttpPatch("doctor")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchDoctor([FromForm] PatchDoctorRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PATCH /api/hospital-management/doctor");

            List<IFormFile> images = new List<IFormFile>();

            if (request.Image != null)
            {
                images.Add(request.Image);
            }

            var payloads = this.GetImagePayload(images);

            var command = request.Adapt<PatchDoctorCammand>() with
            {
                HospNo = base.HospNo,
                HospKey = base.HospKey,
                EmplNo = request.EmplNo,
                DoctNm = request.DoctNm,
                ViewMinCnt = request.ViewMinCnt,
                ViewMinTime = request.ViewMinTime,
                Image = payloads == null || payloads.Count == 0 ? null : payloads[0]
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원정보관리 > 의료진관리]비대면 의료진 상세 API
        /// </summary>
        [HttpGet("doctor-untact/{emplNo}")]
        [ProducesResponseType(typeof(GetDoctorUntactResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDoctorUntact(string emplNo, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET /api/hospital-management/doctor-untact");

            var query = new GetDoctorUntactQuery()
            {
                HospNo = base.HospNo,
                EmplNo = emplNo
            };

            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원정보관리 > 의료진관리]비대면 의료진 수정 API
        /// </summary>
        [HttpPatch("doctor-untact")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchDoctorUntact(PatchDoctorUntactRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PATCH /api/hospital-management/doctor-untact");

            var command = request.Adapt<PatchDoctorUntactCammand>() with
            {
                HospNo = base.HospNo,
                EmplNo = request.EmplNo,
                DoctIntro = request.DoctIntro,
                ClinicGuide = request.ClinicGuide,
                DoctHistoryList = request.DoctHistoryList
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원정보관리 > 의료진관리]주간 스케쥴 수정 API
        /// </summary>
        [HttpPatch("doctor/weeks-schedule")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchDoctorWeeksSchedule(PatchDoctorWeeksScheduleRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PATCH /api/hospital-management/doctor/weeks-schedule");

            var command = new PatchDoctorWeeksScheduleCommand()
            {
                HospNo = base.HospNo,
                HospKey = base.HospKey,
                DoctNo = _cryptoService.EncryptWithNoVector(request.DoctNo),
                DoctNm = request.DoctNm,
                DeptCd = request.DeptCd,
                DeptNm = request.DeptNm,
                ViewRole = request.ViewRole,
                ViewMinTime = request.ViewMinTime,
                ViewMinCnt = request.ViewMinCnt,
                DoctorScheduleList = new List<Modules.Admin.Application.Features.HospitalManagement.Commands.PatchDoctorWeeksScheduleInfo>()
            };

            foreach (var info in request.DoctorScheduleList)
            {
                var doctorScheduleInfo = info.Adapt<Modules.Admin.Application.Features.HospitalManagement.Commands.PatchDoctorWeeksScheduleInfo>() with
                {
                    HospNo = base.HospNo,
                    HospKey = base.HospKey,
                    DoctNo = _cryptoService.EncryptWithNoVector(request.DoctNo),
                    DoctNm = request.DoctNm,
                    DeptCd = request.DeptCd,
                    DeptNm = request.DeptNm,
                    ViewRole = request.ViewRole,
                    ViewMinTime = request.ViewMinTime,
                    ViewMinCnt = request.ViewMinCnt
                };

                command.DoctorScheduleList.Add(doctorScheduleInfo);
            }

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원정보관리 > 의료진관리]지정 스케쥴 수정 API
        /// </summary>
        [HttpPatch("doctor/days-schedule")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchDoctorDaysSchedule(PatchDoctorDaysScheduleRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PATCH /api/hospital-management/doctor/days-schedule");

            var command = new PatchDoctorDaysScheduleCommand()
            {
                HospNo = base.HospNo,
                HospKey = base.HospKey,
                DoctNo = _cryptoService.EncryptWithNoVector(request.DoctNo),
                DoctNm = request.DoctNm,
                DeptCd = request.DeptCd,
                DeptNm = request.DeptNm,
                ViewRole = request.ViewRole,
                ViewMinTime = request.ViewMinTime,
                ViewMinCnt = request.ViewMinCnt,
                DoctorScheduleList = new List<Modules.Admin.Application.Features.HospitalManagement.Commands.PatchDoctorDaysScheduleInfo>()
            };

            foreach (var info in request.DoctorScheduleList)
            {
                var doctorScheduleInfo = info.Adapt<Modules.Admin.Application.Features.HospitalManagement.Commands.PatchDoctorDaysScheduleInfo>() with
                {
                    HospNo = base.HospNo,
                    HospKey = base.HospKey,
                    DoctNo = _cryptoService.EncryptWithNoVector(request.DoctNo),
                    DoctNm = request.DoctNm,
                    DeptCd = request.DeptCd,
                    DeptNm = request.DeptNm,
                    ViewRole = request.ViewRole,
                    ViewMinTime = request.ViewMinTime,
                    ViewMinCnt = request.ViewMinCnt
                };

                command.DoctorScheduleList.Add(doctorScheduleInfo);
            }

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원정보관리 > 의료진관리]비대면 스케쥴 수정 API
        /// </summary>
        [HttpPatch("doctor/untact-weeks-schedule")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchDoctorUntactWeeksSchedule(PatchDoctorUntactWeeksScheduleRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PATCH /api/hospital-management/doctor/untact-weeks-schedule");

            var command = new PatchDoctorUntactWeeksScheduleCommand()
            {
                HospNo = base.HospNo,
                HospKey = base.HospKey,
                DoctNo = _cryptoService.EncryptWithNoVector(request.DoctNo),
                DoctNm = request.DoctNm,
                DeptCd = request.DeptCd,
                DeptNm = request.DeptNm,
                ViewRole = request.ViewRole,
                ViewMinTime = request.ViewMinTime,
                ViewMinCnt = request.ViewMinCnt,
                DoctorScheduleList = new List<Modules.Admin.Application.Features.HospitalManagement.Commands.PatchDoctorUntactWeeksScheduleInfo>()
            };

            foreach (var info in request.DoctorScheduleList)
            {
                var doctorScheduleInfo = info.Adapt<Modules.Admin.Application.Features.HospitalManagement.Commands.PatchDoctorUntactWeeksScheduleInfo>() with
                {
                    HospNo = base.HospNo,
                    HospKey = base.HospKey,
                    DoctNo = _cryptoService.EncryptWithNoVector(request.DoctNo),
                    DoctNm = request.DoctNm,
                    DeptCd = request.DeptCd,
                    DeptNm = request.DeptNm,
                    ViewRole = request.ViewRole,
                    ViewMinTime = request.ViewMinTime,
                    ViewMinCnt = request.ViewMinCnt
                };

                command.DoctorScheduleList.Add(doctorScheduleInfo);
            }

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원정보관리 > 의료진관리 > 주간 스케쥴 관리]예약 정보 API
        /// </summary>
        [HttpPost("doctor/weeks-schedule/reservation")]
        [ProducesResponseType(typeof(GetDoctorWeeksReservationListResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDoctorWeeksReservationList(GetDoctorWeeksReservationListRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/hospital-management/doctor/weeks-schedule/reservation");

            var command = new GetDoctorWeeksReservationListQuery()
            {
                HospNo = base.HospNo,
                EmplNo = request.EmplNo,
                WeekNum = request.WeekNum,
                ReCalculateYn = request.ReCalculateYn,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                BreakStartTime = request.BreakStartTime,
                BreakEndTime = request.BreakEndTime,
                RsrvIntervalTime = request.RsrvIntervalTime,
                RsrvIntervalCnt = request.RsrvIntervalCnt
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원정보관리 > 의료진관리 > 지정 스케쥴 관리]예약 정보 API
        /// </summary>
        [HttpPost("doctor/days-schedule/reservation")]
        [ProducesResponseType(typeof(GetDoctorDaysReservationListResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDoctorDaysReservationList(GetDoctorDaysReservationListRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/hospital-management/doctor/days-schedule/reservation");

            var command = new GetDoctorDaysReservationListQuery()
            {
                HospNo = base.HospNo,
                EmplNo = request.EmplNo,
                ClinicYmd = request.ClinicYmd,
                ReCalculateYn = request.ReCalculateYn,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                BreakStartTime = request.BreakStartTime,
                BreakEndTime = request.BreakEndTime,
                RsrvIntervalTime = request.RsrvIntervalTime,
                RsrvIntervalCnt = request.RsrvIntervalCnt
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원정보관리 > 의료진관리 > 비대면 스케쥴 관리]예약 정보 API
        /// </summary>
        [HttpPost("doctor/untact-weeks-schedule/reservation")]
        [ProducesResponseType(typeof(GetDoctorUntactWeeksReservationListResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDoctorUntactWeeksScheduleReservation(GetDoctorUntactWeeksReservationListRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/hospital-management/doctor/untact-weeks-schedule/reservation");

            var command = new GetDoctorUntactWeeksReservationListQuery()
            {
                HospNo = base.HospNo,
                EmplNo = request.EmplNo,
                WeekNum = request.WeekNum,
                ReCalculateYn = request.ReCalculateYn,
                UntactStartTime = request.UntactStartTime,
                UntactEndTime = request.UntactEndTime,
                UntactBreakStartTime = request.UntactBreakStartTime,
                UntactBreakEndTime = request.UntactBreakEndTime,
                UntactRsrvIntervalTime = request.UntactRsrvIntervalTime,
                UntactRsrvIntervalCnt = request.UntactRsrvIntervalCnt
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원정보관리 > 의료진관리 > 주간 스케쥴 관리]예약 저장 API
        /// </summary>
        [HttpPatch("doctor/weeks-schedule/reservation")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostDoctorWeeksReservation(PostDoctorWeeksReservationRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PATCH /api/hospital-management/doctor/weeks-schedule/reservation");

            var command = new PostDoctorWeeksReservationCommand()
            {
                HospNo = base.HospNo,
                EmplNo = request.EmplNo,
                WeekNum = request.WeekNum,
                RsrvIntervalTime = request.RsrvIntervalTime,
                RsrvIntervalCnt = request.RsrvIntervalCnt,
                EghisDoctRsrvDetailInfoList = request.EghisDoctRsrvDetailInfoList
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원정보관리 > 의료진관리 > 지정 스케쥴 관리]예약 저장 API
        /// </summary>
        [HttpPatch("doctor/days-schedule/reservation")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostDoctorDaysReservation(PostDoctorDaysReservationRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PATCH /api/hospital-management/doctor/days-schedule/reservation");

            var command = new PostDoctorDaysReservationCommand()
            {
                HospNo = base.HospNo,
                EmplNo = request.EmplNo,
                ClinicYmd = request.ClinicYmd,
                RsrvIntervalTime = request.RsrvIntervalTime,
                RsrvIntervalCnt = request.RsrvIntervalCnt,
                EghisDoctRsrvDetailInfoList = request.EghisDoctRsrvDetailInfoList
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원정보관리 > 의료진관리 > 비대면 스케쥴 관리]예약 저장 API
        /// </summary>
        [HttpPatch("doctor/untact-weeks-schedule/reservation")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostDoctorUntactWeeksScheduleReservation(PostDoctorUntactWeeksReservationRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PATCH /api/hospital-management/doctor/untact-weeks-schedule/reservation");

            var command = new PostDoctorUntactWeeksReservationCommand()
            {
                HospNo = base.HospNo,
                EmplNo = request.EmplNo,
                WeekNum = request.WeekNum,
                UntactRsrvIntervalTime = request.UntactRsrvIntervalTime,
                UntactRsrvIntervalCnt = request.UntactRsrvIntervalCnt,
                UntactAvaStartTime = request.UntactAvaStartTime,
                UntactAvaEndTime = request.UntactAvaEndTime,
                UntactAvaUseYn = request.UntactAvaUseYn,
                EghisDoctRsrvDetailInfoList = request.EghisDoctRsrvDetailInfoList
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원정보관리 > 의료진관리]의료진 진료 과목 설정 저장 API
        /// </summary>
        [HttpPost("doctor/medical")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostDoctorMedical(PostDoctorMedicalRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/hospital-management/doctor/medical");

            var eghisDoctInfoMdList = new List<EghisDoctInfoMdEntity>();

            foreach (var mdCd in request.MdCdList)
            {
                var eghisDoctInfoMdEntity = new EghisDoctInfoMdEntity()
                {
                    MdCd = mdCd
                };

                eghisDoctInfoMdList.Add(eghisDoctInfoMdEntity);
            }

            var command = new PostDoctorMedicalCommand()
            {
                HospNo = base.HospNo,
                HospKey = base.HospKey,
                EmplNo = request.EmplNo,
                EghisDoctInfoMdList = eghisDoctInfoMdList
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [병원정보관리 > 의료진관리]비대면 의료진 신청 API
        /// </summary>
        [HttpPost("doctor-untact/join")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PostDoctorUntactJoin([FromForm] PostDoctorUntactJoinRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("POST /api/hospital-management/doctor-untact/join");

            request.HospNo = base.HospNo;

            List<IFormFile> images = new List<IFormFile>();

            if (request.DoctLicenseFile != null)
            {
                images.Add(request.DoctLicenseFile);
            }

            if (request.DoctFile != null)
            {
                images.Add(request.DoctFile);
            }

            if (request.AccountInfoFile != null)
            {
                images.Add(request.AccountInfoFile);
            }

            if (request.BusinessFile != null)
            {
                images.Add(request.BusinessFile);
            }

            var payloads = this.GetImagePayload(images);

            var command = new PostDoctorUntactJoinCommand()
            {
                HospNo = base.HospNo,
                EmplNo = request.EmplNo,
                DoctNo = request.DoctNo,
                DoctNoType = request.DoctNoType,
                DoctNm = request.DoctNm,
                DoctBirthday = request.DoctBirthday,
                DoctTel = request.DoctTel,
                DoctIntro = request.DoctIntro,
                DoctHistoryList = request.DoctHistoryList,
                ClinicTime = request.ClinicTime,
                ClinicGuide = request.ClinicGuide,
                Images = payloads
            };

            var result = await _mediator.Send(command, cancellationToken);

            return result.ToActionResult(this);
        }

        #region INTERNAL METHOD AREA ********************************************
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

        private int SetRole(List<int>? roleList)
        {
            if (roleList == null || roleList.Count <= 0)
                return 0;

            int role = 0;

            foreach (var item in roleList)
            {
                role |= item;
            }

            return role;
        }

        private string? CustomJsonToString(DeviceSettingInfo settingInfo)
        {
            if (settingInfo == null)
                return null;

            return JsonSerializer.Serialize(settingInfo,
                new JsonSerializerOptions
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
        }
        #endregion
    }
}
