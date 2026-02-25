using Hello100Admin.BuildingBlocks.Common.Application;
using Hello100Admin.BuildingBlocks.Common.Definition.Enums;
using Hello100Admin.BuildingBlocks.Common.Infrastructure.Persistence.Core;
using Hello100Admin.Modules.Admin.Domain.Entities;
using Hello100Admin.Modules.Admin.Domain.Repositories;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Hello100Admin.Modules.Admin.Application.Features.HospitalManagement.Commands
{
    public record BulkUpdateSymptomExamKeywordsCommand(List<BulkUpdateSymptomExamKeywordsCommandItem> Items) : IQuery<Result>;

    public sealed record BulkUpdateSymptomExamKeywordsCommandItem
    {
        /// <summary>
        /// 대표 키워드 고유번호
        /// </summary>
        public int MasterSeq { get; init; }
        /// <summary>
        /// 정렬 순서
        /// </summary>
        public int SortNo { get; init; }
        /// <summary>
        /// 노출 여부
        /// </summary>
        public string ShowYn { get; init; } = default!;
    }

    public class BulkUpdateSymptomExamKeywordsCommandHandler : IRequestHandler<BulkUpdateSymptomExamKeywordsCommand, Result>
    {
        private readonly ILogger<BulkUpdateSymptomExamKeywordsCommandHandler> _logger;
        private readonly IHospitalManagementRepository _hospitalManagementRepository;
        private readonly IDbSessionRunner _db;

        public BulkUpdateSymptomExamKeywordsCommandHandler(
            ILogger<BulkUpdateSymptomExamKeywordsCommandHandler> logger,
            IHospitalManagementRepository hospitalManagementRepository,
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalManagementRepository = hospitalManagementRepository;
            _db = db;
        }

        public async Task<Result> Handle(BulkUpdateSymptomExamKeywordsCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handling BulkUpdateSymptomExamKeywordsCommandHandler");

            var keywordEntities = req.Items.Adapt<List<TbKeywordMasterEntity>>();

            await _db.RunAsync(DataSource.Hello100,
                (session, token) => _hospitalManagementRepository.BulkUpdateSymptomExamKeywordsAsync(session, keywordEntities, token),
            ct);

            return Result.Success();
        }
    }
}
