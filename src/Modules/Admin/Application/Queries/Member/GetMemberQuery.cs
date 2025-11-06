using MediatR;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.DTOs;

namespace Hello100Admin.Modules.Admin.Application.Queries.Member;

/// <summary>
/// 멤버 상세 조회 Query
/// </summary>
public record GetMemberQuery(string Uid) : IRequest<Result<MemberDto>>;