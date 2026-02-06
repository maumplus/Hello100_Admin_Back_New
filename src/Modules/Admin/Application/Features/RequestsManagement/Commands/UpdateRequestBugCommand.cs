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
    /// <summary>
    /// 잘못된 정보 관리자확인 업데이트 커맨드
    /// </summary>
    public record UpdateRequestBugCommand : IQuery<Result>
    {
        public int HpId { get; init; }
        public string ApprAid { get; init; } = default!;
    }

    public class UpdateRequestBugCommandValidator : AbstractValidator<UpdateRequestBugCommand>
    {
        public UpdateRequestBugCommandValidator()
        {
            RuleFor(x => x.HpId).NotNull().WithMessage("요청ID는 필수입니다.");
        }
    }

    public class UpdateRequestBugCommandHandler : IRequestHandler<UpdateRequestBugCommand, Result>
    {
        private readonly ILogger<UpdateRequestBugCommandHandler> _logger;
        private readonly IRequestsManagementRepository _requestsManagementRepository;
        private readonly IDbSessionRunner _db;

        public UpdateRequestBugCommandHandler(
            ILogger<UpdateRequestBugCommandHandler> logger,
            IRequestsManagementRepository requestsManagementRepository,
            IDbSessionRunner db)
        {
            _logger = logger;
            _requestsManagementRepository = requestsManagementRepository;
            _db = db;
        }

        public async Task<Result> Handle(UpdateRequestBugCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handling UpdateRequestBugCommand for HpId: {HpId}", req.HpId);

            var tbHospitalProposalInfoEntity = new TbHospitalProposalInfoEntity
            {
                Hpid = req.HpId,
                ApprAid = req.ApprAid
            };

            await _db.RunAsync(DataSource.Hello100, 
                (dbSession, token) => _requestsManagementRepository.UpdateRequestBugAsync(dbSession, tbHospitalProposalInfoEntity, token),
                ct);
            
            return Result.Success();
        }
    }
}
