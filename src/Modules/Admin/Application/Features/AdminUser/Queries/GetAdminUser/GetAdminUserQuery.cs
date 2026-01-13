using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Responses.GetAdminUser;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Responses.Shared;

namespace Hello100Admin.Modules.Admin.Application.Features.AdminUser.Queries.GetAdminUser;

/// <summary>
/// 관리자 사용자 목록 조회 Query
/// </summary>
public record GetAdminUserQuery(
    int Page = 1,
    int PageSize = 20,
    bool IncludeDeleted = false
) : IQuery<Result<PagedResult<AdminUserResponse>>>;
