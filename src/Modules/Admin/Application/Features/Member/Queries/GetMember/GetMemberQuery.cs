using MediatR;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.Modules.Admin.Application.Features.Member.Responses.GetMember;

namespace Hello100Admin.Modules.Admin.Application.Features.Member.Queries.GetMember;

/// <summary>
/// 멤버 상세 조회 Query
/// </summary>
public record GetMemberQuery(string Uid) : IRequest<Result<GetMemberResponse>>;