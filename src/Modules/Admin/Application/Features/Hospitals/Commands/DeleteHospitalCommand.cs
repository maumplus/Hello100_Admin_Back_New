using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Abstractions.Persistence.Common;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.Hospitals.Commands
{
    public record DeleteHospitalCommand(string HospKey) : IQuery<Result>;

    public class DeleteHospitalCommandHandler : IRequestHandler<DeleteHospitalCommand, Result>
    {
        private readonly ILogger<DeleteHospitalCommandHandler> _logger;
        private readonly IHospitalsRepository _hospitalsRepository;
        private readonly IHospitalInfoProvider _hospitalInfoProvider;
        private readonly IDbSessionRunner _db;

        public DeleteHospitalCommandHandler(
            ILogger<DeleteHospitalCommandHandler> logger,
            IHospitalsRepository hospitalsRepository,
            IHospitalInfoProvider hospitalInfoProvider,
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalsRepository = hospitalsRepository;
            _hospitalInfoProvider = hospitalInfoProvider;
            _db = db;
        }

        public async Task<Result> Handle(DeleteHospitalCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handle CreateHospitalCommandHandler");

            var hospInfo = await _hospitalInfoProvider.GetHospitalInfoByHospKeyAsync(req.HospKey, ct);

            await _db.RunAsync(DataSource.Hello100,
                (session, token) => _hospitalsRepository.DeleteHospitalAsync(session, hospInfo?.HospNo, req.HospKey, token),
            ct);

            return Result.Success();
        }
    }
}
