using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.Departments.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Departments.Queries
{
    public record GetDepartmentsQuery() : IQuery<Result<ListResult<GetDepartmentsResult>>>;

    public class GetDepartmentsQueryQueryHandler : IRequestHandler<GetDepartmentsQuery, Result<ListResult<GetDepartmentsResult>>>
    {
        private readonly ILogger<GetDepartmentsQueryQueryHandler> _logger;
        private readonly IDepartmentsStore _departmentsStore;
        private readonly IDbSessionRunner _db;

        public GetDepartmentsQueryQueryHandler(
            ILogger<GetDepartmentsQueryQueryHandler> logger,
            IDepartmentsStore departmentsStore,
            IDbSessionRunner db)
        {
            _logger = logger;
            _departmentsStore = departmentsStore;
            _db = db;
        }

        public async Task<Result<ListResult<GetDepartmentsResult>>> Handle(GetDepartmentsQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetDepartmentsQuery");

            var result = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _departmentsStore.GetDepartmentsAsync(session, "03", token),
            ct);

            return Result.Success(result);
        }
    }
}
