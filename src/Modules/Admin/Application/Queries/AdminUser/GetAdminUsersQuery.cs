using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.DTOs;

namespace Hello100Admin.Modules.Admin.Application.Queries;

/// <summary>
/// 관리자 사용자 목록 조회 Query
/// </summary>
public record GetAdminUsersQuery(
    int Page = 1,
    int PageSize = 20,
    bool IncludeDeleted = false
) : IQuery<Result<PagedResult<AdminUserListDto>>>;
