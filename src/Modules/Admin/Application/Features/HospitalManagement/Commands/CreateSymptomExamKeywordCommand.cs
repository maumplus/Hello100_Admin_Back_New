using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Commands
{
    public record CreateSymptomExamKeywordCommand : IQuery<Result>
    {
        /// <summary>
        /// 대표 키워드명
        /// </summary>
        public string MasterName { get; init; } = default!;
        /// <summary>
        /// 노출 여부
        /// </summary>
        public string ShowYn { get; init; } = default!;
        /// <summary>
        /// 상세 키워드 사용 여부
        /// </summary>
        public string DetailUseYn { get; init; } = default!;
        /// <summary>
        /// 상세 키워드명 리스트
        /// </summary>
        public List<string> DetailNames { get; init; } = default!;
    }

    public class CreateSymptomExamKeywordCommandHandler : IRequestHandler<CreateSymptomExamKeywordCommand, Result>
    {
        private readonly ILogger<CreateSymptomExamKeywordCommandHandler> _logger;
        private readonly IHospitalManagementRepository _hospitalManagementRepository;
        private readonly IDbSessionRunner _db;

        public CreateSymptomExamKeywordCommandHandler(
            ILogger<CreateSymptomExamKeywordCommandHandler> logger, 
            IHospitalManagementRepository hospitalManagementRepository, 
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalManagementRepository = hospitalManagementRepository;
            _db = db;
        }

        public async Task<Result> Handle(CreateSymptomExamKeywordCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handle CreateSymptomExamKeywordCommandHandler");

            await _db.RunAsync(DataSource.Hello100,
                (session, token) => _hospitalManagementRepository.CreateSymptomExamKeywordAsync(session, req.MasterName, req.ShowYn, req.DetailUseYn, req.DetailNames, token),
            ct);

            return Result.Success();
        }
    }
}
