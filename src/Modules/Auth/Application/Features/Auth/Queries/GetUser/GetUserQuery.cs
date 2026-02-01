using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.GetUser;
using MediatR;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Queries.GetUser;

/// <summary>
/// 사용자 조회 쿼리
/// </summary>
public class GetUserQuery : IRequest<Result<UserResponse>>
{
    public string Aid { get; set; } = string.Empty;
}
