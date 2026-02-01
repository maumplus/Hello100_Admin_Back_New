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
    public record DeleteUserCommand(string UserId) : IQuery<Result>;

    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
    {
        public DeleteUserCommandValidator()
        {
            RuleFor(x => x.UserId)
                .Must(x => string.IsNullOrWhiteSpace(x) == false).WithMessage("사용자 ID는 필수입니다.");
        }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
    {
        private readonly ILogger<DeleteUserCommandHandler> _logger;
        private readonly IHospitalUserRepository _hospitalUserRepository;
        private readonly IDbSessionRunner _db;

        public DeleteUserCommandHandler(
            ILogger<DeleteUserCommandHandler> logger,
            IHospitalUserRepository hospitalUserRepository,
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalUserRepository = hospitalUserRepository;
            _db = db;
        }

        public async Task<Result> Handle(DeleteUserCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handling DeleteUserCommand for UserId: {UserId}", req.UserId);
            
            await _db.RunAsync(DataSource.Hello100, async dbSession =>
            {
                await _hospitalUserRepository.DeleteUserAsync(dbSession, req.UserId, ct);
            }, ct);
            
            return Result.Success();
        }
    }
}
