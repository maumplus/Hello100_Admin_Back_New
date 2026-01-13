using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.AdminUser;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Responses.GetAdminUser;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Responses.Shared;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.AdminUser.Queries.GetAdminUser;

public class GetAdminUserQueryHandler : IRequestHandler<GetAdminUserQuery, Result<PagedResult<AdminUserResponse>>>
{
    private readonly IAdminUserRepository _repository;
    private readonly IAdminUserStore _store;
    private readonly ILogger<GetAdminUserQueryHandler> _logger;

    public GetAdminUserQueryHandler(
        IAdminUserRepository repository,
        IAdminUserStore store,
        ILogger<GetAdminUserQueryHandler> logger)
    {
        _repository = repository;
        _store = store;
        _logger = logger;
    }

    public async Task<Result<PagedResult<AdminUserResponse>>> Handle(GetAdminUserQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting admin users: Page={Page}, PageSize={PageSize}", query.Page, query.PageSize);

        var (items, totalCount) = await _store.GetPagedAsync(
            query.Page,
            query.PageSize,
            includeDeleted: query.IncludeDeleted,
            cancellationToken);

        // Grade 기반 역할 설정
        var roleNames = items.Select(c => GetRoleNameByGrade(c.Grade)).Distinct();

        var dtos = items.Select(au => new AdminUserResponse
        {
            AccountId = au.AccId,
            Name = au.Name,
            Tel = au.Tel,
            HospNo = au.HospNo,
            Enabled = au.DelYn == "N",
            Approved = au.Approved == "1",
            CreatedAt = au.RegDt,
            LastLoginAt = au.LastLoginAt,
            Roles = roleNames.ToList()
        }).ToList();

        var result = new PagedResult<AdminUserResponse>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };

        _logger.LogInformation("Retrieved {Count} admin users out of {Total}", dtos.Count, totalCount);

        return Result.Success(result);
    }

    private string GetRoleNameByGrade(string grade) => grade switch
    {
        "S" => "SuperAdmin",
        "C" => "HospitalAdmin",
        "A" => "GeneralAdmin",
        _ => "GeneralAdmin"
    };
}
