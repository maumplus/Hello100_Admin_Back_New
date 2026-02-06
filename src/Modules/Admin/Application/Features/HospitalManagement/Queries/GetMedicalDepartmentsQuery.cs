using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Hospital;
using Hello100Admin.Modules.Admin.Application.Common.Models;
using Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Results;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Queries
{
    public record GetMedicalDepartmentsQuery() : IQuery<Result<ListResult<GetMedicalDepartmentsResult>>>;

    public class GetMedicalDepartmentsQueryHandler : IRequestHandler<GetMedicalDepartmentsQuery, Result<ListResult<GetMedicalDepartmentsResult>>>
    {
        private readonly ILogger<GetMedicalDepartmentsQueryHandler> _logger;
        private readonly IHospitalManagementStore _hospitalStore;
        private readonly IDbSessionRunner _db;

        public GetMedicalDepartmentsQueryHandler(
            ILogger<GetMedicalDepartmentsQueryHandler> logger,
            IHospitalManagementStore hospitalStore,
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalStore = hospitalStore;
            _db = db;
        }

        public async Task<Result<ListResult<GetMedicalDepartmentsResult>>> Handle(GetMedicalDepartmentsQuery req, CancellationToken ct)
        {
            _logger.LogInformation("Handling GetMedicalDepartmentsQuery");

            var result = await _db.RunAsync(DataSource.Hello100,
                (session, token) => _hospitalStore.GetMedicalDepartmentsAsync(session, "03", token),
            ct);

            return Result.Success(result);
        }
    }
}
