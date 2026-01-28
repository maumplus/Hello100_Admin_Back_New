using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Member;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Infrastructure.Repositories.Member;

public class MemberRepository : IMemberRepository
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<MemberRepository> _logger;

    public MemberRepository(IDbConnectionFactory connectionFactory, ILogger<MemberRepository> logger)
    {
        _connectionFactory = connectionFactory;
        _logger = logger;
    }
}