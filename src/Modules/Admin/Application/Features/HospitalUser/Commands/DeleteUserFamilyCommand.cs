using FluentValidation;
using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalUser.Commands
{
    /// <summary>
    /// 회원 가족 삭제 커맨드
    /// </summary>
    /// <param name="UserId">사용자 ID</param>
    /// <param name="MId">가족 ID</param>
    public record DeleteUserFamilyCommand(string UserId, int MId) : IQuery<Result>;

    public class DeleteUserFamilyCommandValidator : AbstractValidator<DeleteUserFamilyCommand>
    {
        public DeleteUserFamilyCommandValidator()
        {
            RuleFor(x => x.UserId)
                .Must(x => string.IsNullOrWhiteSpace(x) == false).WithMessage("사용자 ID는 필수입니다.");
            RuleFor(x => x.MId)
                .NotNull().GreaterThan(0).WithMessage("가족 ID는 0보다 커야 합니다.");
        }
    }

    public class DeleteUserFamilyCommandHandler : IRequestHandler<DeleteUserFamilyCommand, Result>
    {
        private readonly ILogger<DeleteUserFamilyCommandHandler> _logger;
        private readonly IHospitalUserRepository _hospitalUserRepository;
        private readonly IDbSessionRunner _db;

        public DeleteUserFamilyCommandHandler(
            ILogger<DeleteUserFamilyCommandHandler> logger,
            IHospitalUserRepository hospitalUserRepository,
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalUserRepository = hospitalUserRepository;
            _db = db;
        }

        public async Task<Result> Handle(DeleteUserFamilyCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handling DeleteUserFamilyCommand for UserId: {UserId}, MId: {MId}", req.UserId, req.MId);
            
            await _db.RunAsync(DataSource.Hello100, async dbSession =>
            {
                await _hospitalUserRepository.DeleteUserFamilyAsync(dbSession, req.UserId, req.MId, ct);
            }, ct);
            
            return Result.Success();
        }
    }
}
