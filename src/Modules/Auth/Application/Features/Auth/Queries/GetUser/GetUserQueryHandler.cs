using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Auth.Application.Common.Abstractions.Persistence.Auth;
using Hello100Admin.Modules.Auth.Application.Features.Auth.Responses.GetUser;
using MediatR;

namespace Hello100Admin.Modules.Auth.Application.Features.Auth.Queries.GetUser;

/// <summary>
/// 사용자 조회 쿼리 핸들러
/// </summary>
public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<UserResponse>>
{
    private readonly IAuthStore _authStore;

    public GetUserQueryHandler(IAuthStore authStore)
    {
        _authStore = authStore;
    }

    public async Task<Result<UserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _authStore.GetAdminInfoByAIdAsync(request.AId, cancellationToken);
        
        if (user == null)
        {
            // 임시 적용
            return Result.Success<UserResponse>().WithError(new ErrorInfo(0, "UserNotFound", "사용자를 찾을 수 없습니다."));
        }

        // Grade 기반 역할 설정
        var roleNames = new[] { GetRoleNameByGrade(user.Grade) };

        var userDto = new UserResponse
        {
            Id = user.AId,
            AccountId = user.AccId,
            Name = user.Name,
            HospNo = user.HospNo,
            Grade = user.Grade,
            Enabled = user.Enabled == "1",
            Approved = user.Approved == "1",
            AccountLocked = user.AccountLocked == "Y",
            LastLoginAt = user.LastLoginDt,
            Roles = roleNames.ToList()
        };

        return Result.Success(userDto);
    }

    private string GetRoleNameByGrade(string grade) => grade switch
    {
        "S" => "SuperAdmin",
        "C" => "HospitalAdmin",
        "A" => "GeneralAdmin",
        _ => "GeneralAdmin"
    };
}
