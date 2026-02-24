

using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.RequestsManagement.Commands
{
    public record UpdateRequestUntactCommand : IQuery<Result>
    {
        public int Seq { get; set; }
        public string JoinState { get; set; } = default!;
        public string? ReturnReason { get; set; } = default!;
    }

    public class UpdateRequestUntactCommandValidator : AbstractValidator<UpdateRequestUntactCommand>
    {
        public UpdateRequestUntactCommandValidator()
        {
            RuleFor(x => x.Seq).NotNull().WithMessage("요청ID는 필수입니다.");
            RuleFor(x => x.JoinState).NotNull().WithMessage("신청상태는 필수입니다.");
        }
    }

    public class UpdateRequestUntactCommandHandler : IRequestHandler<UpdateRequestUntactCommand, Result>
    {
        private readonly ILogger<UpdateRequestBugCommandHandler> _logger;
        private readonly IRequestsManagementRepository _requestsManagementRepository;
        private readonly IDbSessionRunner _db;

        public UpdateRequestUntactCommandHandler(
            ILogger<UpdateRequestBugCommandHandler> logger,
            IRequestsManagementRepository requestsManagementRepository,
            IDbSessionRunner db)
        {
            _logger = logger;
            _requestsManagementRepository = requestsManagementRepository;
            _db = db;
        }

        public async Task<Result> Handle(UpdateRequestUntactCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handling UpdateRequestUntactCommand for Seq: {Seq}", req.Seq);

            var tbEghisDoctUntactJoinEntity = new TbEghisDoctUntactJoinEntity
            {
                Seq = req.Seq,
                JoinState = req.JoinState,
                ReturnReason = req.ReturnReason
            };

            await _db.RunAsync(DataSource.Hello100,
                (dbSession, token) => _requestsManagementRepository.UpdateRequestUntactAsync(dbSession, tbEghisDoctUntactJoinEntity, token),
                ct);

            return Result.Success();
        }
    }
}
