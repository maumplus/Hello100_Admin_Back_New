using Hello100Admin.API.Constracts.Admin.HospitalUser;
using Hello100Admin.API.Extensions;
using Hello100Admin.API.Infrastructure.Attributes;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.HospitalUser.Commands;
using Hello100Admin.Modules.Admin.Application.Features.HospitalUser.Queries;
using Hello100Admin.Modules.Admin.Application.Features.HospitalUser.Results;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Hello100Admin.API.Controllers
{
    /// <summary>
    /// 회원 관리(병원 사용자 관리) API Controller
    /// </summary>
    [Auth]
    [Route("api/hospital-user")]
    public class HospitalUserController : BaseController
    {
        private readonly ILogger<HospitalUserController> _logger;
        private readonly IMediator _mediator;

        public HospitalUserController(IMediator mediator, ILogger<HospitalUserController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// [전체관리자] 회원목록 > 조회
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<ListResult<SearchHospitalUsersResult>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchHospitalUsers([FromQuery] SearchHospitalUsersRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET api/hospital-user [{Aid}]", Aid);

            var query = req.Adapt<SearchHospitalUsersQuery>();

            var result = await _mediator.Send(query, cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체관리자] 회원목록 > 상세 조회 (상세 정보, 가족 프로필 정보, 서비스 이용 현황 조회)
        /// </summary>
        [HttpGet("{userId}/profile")]
        [ProducesResponseType(typeof(ApiResponse<GetHospitalUserProfileResult>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetHospitalUserProfile(string userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GET api/hospital-user/{userId}/profile [{Aid}]", userId, Aid);

            var result = await _mediator.Send(new GetHospitalUserProfileQuery(userId), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체관리자] 회원목록 > 조회 > 특정 회원 클릭 > 상세 조회 > 사용자 권한 수정 및 저장
        /// </summary>
        [HttpPatch("{userId}/role")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateHospitalUserRole(string userId, UpdateHospitalUserRoleRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("PATCH api/hospital-user/{userId}/role [{Aid}]", userId, Aid);

            var result = await _mediator.Send(new UpdateHospitalUserRoleCommand(userId, req.UserRole), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체관리자] 회원목록 > 조회 > 특정 회원 클릭 > 상세 조회 > 프로필 삭제 (가족 삭제)
        /// </summary>
        [HttpDelete("{userId}/family/{mId}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUserFamily(string userId, int mId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("DELETE api/hospital-user/{userId}/family/{mId} [{Aid}]", userId, mId, Aid);

            var result = await _mediator.Send(new DeleteUserFamilyCommand(userId, mId), cancellationToken);

            return result.ToActionResult(this);
        }

        /// <summary>
        /// [전체관리자] 회원목록 > 조회 > 특정 회원 클릭 > 상세 조회 > 회원 삭제
        /// </summary>
        [HttpDelete("{userId}")]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteUser(string userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("DELETE api/hospital-user/{userId} [{Aid}]", userId, Aid);

            var result = await _mediator.Send(new DeleteUserCommand(userId), cancellationToken);

            return result.ToActionResult(this);
        }
    }
}
