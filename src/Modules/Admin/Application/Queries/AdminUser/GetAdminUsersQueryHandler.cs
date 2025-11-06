using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Application.DTOs;
using Hello100Admin.Modules.Admin.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Queries;

public class GetAdminUsersQueryHandler : IRequestHandler<GetAdminUsersQuery, Result<PagedResult<AdminUserListDto>>>
{
    private readonly IAdminUserRepository _repository;
    private readonly ILogger<GetAdminUsersQueryHandler> _logger;

    public GetAdminUsersQueryHandler(
        IAdminUserRepository repository,
        ILogger<GetAdminUsersQueryHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Result<PagedResult<AdminUserListDto>>> Handle(
        GetAdminUsersQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting admin users: Page={Page}, PageSize={PageSize}", query.Page, query.PageSize);

        var (items, totalCount) = await _repository.GetPagedAsync(
            query.Page,
            query.PageSize,
            includeDeleted: query.IncludeDeleted,
            cancellationToken);

        // Grade 기반 역할 설정
        var roleNames = items.Select(c => GetRoleNameByGrade(c.Grade)).Distinct();

        var dtos = items.Select(au => new AdminUserListDto
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

        var result = new PagedResult<AdminUserListDto>
        {
            Items = dtos,
            TotalCount = totalCount,
            Page = query.Page,
            PageSize = query.PageSize
        };

        _logger.LogInformation("Retrieved {Count} admin users out of {Total}", dtos.Count, totalCount);

        return Result<PagedResult<AdminUserListDto>>.Success(result);
    }

    private string GetRoleNameByGrade(string grade) => grade switch
    {
        "S" => "SuperAdmin",
        "C" => "HospitalAdmin",
        "A" => "GeneralAdmin",
        _ => "GeneralAdmin"
    };
}
