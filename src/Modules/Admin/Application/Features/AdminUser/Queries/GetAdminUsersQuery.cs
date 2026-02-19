using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Serialization;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.AdminUser;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.AdminUser.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.AdminUser.Queries
{
    public record GetAdminUsersQuery(int PageNo, int PageSize) : IQuery<Result<ListResult<GetAdminUsersResult>>>;

    public class GetAdminUsersQueryValidator : AbstractValidator<GetAdminUsersQuery>
    {
        public GetAdminUsersQueryValidator()
        {
            RuleFor(x => x.PageNo).NotNull().GreaterThan(0).WithMessage("페이지 번호는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.PageSize).NotNull().GreaterThan(0).WithMessage("페이지 사이즈는 필수이며 0보다 커야 합니다.");
        }
    }

    public class GetAdminUsersQueryHandler : IRequestHandler<GetAdminUsersQuery, Result<ListResult<GetAdminUsersResult>>>
    {
        private readonly ILogger<GetAdminUsersQueryHandler> _logger;
        private readonly IAdminUserStore _adminUserStore;
        private readonly IDbSessionRunner _db;

        public GetAdminUsersQueryHandler(
            ILogger<GetAdminUsersQueryHandler> logger,
            IAdminUserStore adminUserStore,
            IDbSessionRunner db)
        {
            _logger = logger;
            _adminUserStore = adminUserStore;
            _db = db;
        }

        public async Task<Result<ListResult<GetAdminUsersResult>>> Handle(GetAdminUsersQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetAdminUsersQuery");

            var response = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _adminUserStore.GetAdminUsersAsync(session, req.PageNo, req.PageSize, token),
            ct);

            var test = response.ToJsonForStorage();

            return Result.Success(response);
        }
    }
}
