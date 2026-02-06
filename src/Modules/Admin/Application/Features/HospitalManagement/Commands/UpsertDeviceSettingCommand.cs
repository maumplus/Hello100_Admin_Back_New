using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Application.Common.Errors;
using Hello100Admin.Modules.Admin.Application.Common.Extensions;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Commands
{
    public record UpsertDeviceSettingCommand : IQuery<Result>
    {
        public string HospNo { get; init; }
        public string HospKey { get; init; }
        public string HospNm { get; init; }
        public string EmplNo { get; init; }
        public string DeviceNm { get; init; }
        public int DeviceType { get; init; }
        public string InfoTxt { get; init; }
        public string UseYn { get; init; }
        public string? SetJson { get; init; }
    }

    public class UpsertDeviceSettingCommandValidator : AbstractValidator<UpsertDeviceSettingCommand>
    {
        public UpsertDeviceSettingCommandValidator()
        {
        }
    }

    public class UpsertDeviceSettingCommandHandler : IRequestHandler<UpsertDeviceSettingCommand, Result>
    {
        private readonly ILogger<UpsertDeviceSettingCommandHandler> _logger;
        private readonly IHospitalManagementRepository _hospitalRepository;
        private readonly IDbSessionRunner _db;

        public UpsertDeviceSettingCommandHandler(
            ILogger<UpsertDeviceSettingCommandHandler> logger,
            IHospitalManagementRepository hospitalRepository,
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalRepository = hospitalRepository;
            _db = db;
        }

        public async Task<Result> Handle(UpsertDeviceSettingCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handling UpsertDeviceSettingCommand");

            if (string.IsNullOrWhiteSpace(req.SetJson) == true)
                return Result.Success().WithError(AdminErrorCode.NoDataToUpsert.ToError());

            var result = await _db.RunAsync(DataSource.Hello100, 
                (session, token) => _hospitalRepository.UpsertDeviceSettingAsync(
                    session, req.HospNo, req.HospNm, req.EmplNo, req.DeviceNm, req.DeviceType, req.HospKey, req.InfoTxt, req.UseYn, req.SetJson, token), 
            ct);

            return Result.Success();
        }
    }
}
