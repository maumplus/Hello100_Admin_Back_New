using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Errors;
using Hello100Admin.Modules.Admin.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Queries.Member;

using Hello100Admin.BuildingBlocks.Common.Infrastructure.Extensions;
using Hello100Admin.Modules.Admin.Application.DTOs;

public class GetMemberQueryHandler : IRequestHandler<GetMemberQuery, Result<MemberDto>>
{
    private readonly IMemberRepository _userRepository;
    private readonly ILogger<GetMemberQueryHandler> _logger;

    public GetMemberQueryHandler(
        IMemberRepository userRepository,
        ILogger<GetMemberQueryHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<MemberDto>> Handle(GetMemberQuery query, CancellationToken cancellationToken)
    {
        var member = await _userRepository.GetMember(query.Uid, cancellationToken);
        if (member == null)
        {
            _logger.LogWarning("Member with UID {Uid} not found.", query.Uid);
            return Result.SuccessWithError<MemberDto>(GlobalErrorCode.UserNotFound.ToError());
        }

        var memberDto = new MemberDto
        {
            Uid = member.Uid,
            Mid = member.Mid,
            Name = member.Name.DecryptedValue,
            SnsId = member.SnsId?.DecryptedValue ?? string.Empty,
            Email = member.Email?.DecryptedValue ?? string.Empty,
            Phone = member.Phone?.DecryptedValue ?? string.Empty,
            LoginType = member.LoginType,
            LoginTypeName = member.LoginTypeName,
            Said = member.Said ?? 0,
            RegDt = (int)new DateTimeOffset(member.RegDt).ToUnixTimeSeconds(),
            RegDtView = member.RegDtView,
            LastLoginDt = (int)new DateTimeOffset(member.LastLoginDt).ToUnixTimeSeconds(),
            LastLoginDtView = member.LastLoginDtView,
            UserRole = member.UserRole
        };

        var memberFamilys = await _userRepository.GetMemberFamilys(query.Uid, cancellationToken);
        foreach (var family in memberFamilys)
        {
            if (family != null)
            {
                memberDto.MemberFamilys.Add(new MemberFamilyDto
                {
                    Uid = family.Uid,
                    Mid = family.Mid,
                    Name = family.Name.DecryptedValue,
                    Birthday = family.Birthday.DecryptedValue.Substring(0, 4) + "년 "+
                               family.Birthday.DecryptedValue.Substring(4, 2) + "월 " +
                               family.Birthday.DecryptedValue.Substring(6, 2) + "일",
                    Sex = family.Sex.DecryptedValue,
                    RegDt = family.RegDt.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
        }

        // 레거시 비즈니스 로직 작업이 더 필요
        // Clinics (회원서비스 현황)

        return Result.Success<MemberDto>(memberDto);
    }
}