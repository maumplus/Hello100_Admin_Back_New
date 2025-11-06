using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Auth.Application.DTOs;
using Hello100Admin.Modules.Auth.Domain.Interfaces;
using MediatR;

namespace Hello100Admin.Modules.Auth.Application.Queries.GetUser;

/// <summary>
/// 사용자 조회 쿼리 핸들러
/// </summary>
public class GetUserQueryHandler : IRequestHandler<GetUserQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;

    public GetUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<UserDto>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByAidAsync(request.UserId, cancellationToken);
        
        if (user == null)
        {
            return Result.Failure<UserDto>("사용자를 찾을 수 없습니다.");
        }

        // Grade 기반 역할 설정
        var roleNames = new[] { GetRoleNameByGrade(user.Grade) };

        var userDto = new UserDto
        {
            Id = user.Aid,
            AccountId = user.AccId,
            Name = user.Name,
            HospNo = user.HospNo,
            Grade = user.Grade,
            Enabled = user.Enabled == "1",
            Approved = user.Approved == "1",
            AccountLocked = user.AccountLocked == "0",
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
