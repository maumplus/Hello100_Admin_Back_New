using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Auth.Application.DTOs;
using MediatR;

namespace Hello100Admin.Modules.Auth.Application.Queries.GetUser;

/// <summary>
/// 사용자 조회 쿼리
/// </summary>
public class GetUserQuery : IRequest<Result<UserDto>>
{
    public string UserId { get; set; } = string.Empty;
}
