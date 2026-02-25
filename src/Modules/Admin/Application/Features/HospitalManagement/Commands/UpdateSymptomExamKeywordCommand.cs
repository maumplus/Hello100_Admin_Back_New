using FluentValidation;
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
    public record UpdateSymptomExamKeywordCommand : IQuery<Result>
    {
        /// <summary>
        /// 대표 키워드 고유번호
        /// </summary>
        public int MasterSeq { get; init; }
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
        /// 상세 키워드 리스트
        /// </summary>
        public List<UpdateSymptomExamKeywordCommandItem> DetailKeywordItems { get; init; } = default!;
    }

    public sealed record UpdateSymptomExamKeywordCommandItem
    {
        /// <summary>
        /// 상세 키워드 고유번호
        /// </summary>
        public int DetailSeq { get; init; }
        /// <summary>
        /// 상세 키워드명
        /// </summary>
        public string DetailName { get; init; } = default!;
    }

    public class UpdateSymptomExamKeywordCommandValidator : AbstractValidator<UpdateSymptomExamKeywordCommand>
    {
        public UpdateSymptomExamKeywordCommandValidator()
        {
            RuleFor(x => x.MasterSeq)
                .NotNull().GreaterThan(0).WithMessage("대표 키워드 고유번호는 0보다 커야 합니다.");
            RuleFor(x => x.MasterName)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("대표 키워드명은 필수입니다.");
            RuleFor(x => x.ShowYn)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("노출 여부는 필수입니다.")
                .Must(x => x == "Y" || x == "N").WithMessage("노출 여부는 'Y' 또는 'N'이어야 합니다.");
            RuleFor(x => x.DetailUseYn)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("상세 키워드 사용 여부는 필수입니다.")
                .Must(x => x == "Y" || x == "N").WithMessage("상세 키워드 사용 여부는 'Y' 또는 'N'이어야 합니다.");
            RuleFor(x => x.DetailKeywordItems)
                .NotNull().WithMessage("상세 키워드 리스트는 필수입니다.")
                .NotEmpty().WithMessage("상세 키워드 리스트가 비어있습니다.");
            RuleForEach(x => x.DetailKeywordItems)
                .ChildRules(x =>
                {
                    x.RuleFor(x => x.DetailSeq)
                         .NotNull().GreaterThanOrEqualTo(0).WithMessage("상세 키워드 고유번호는 0보다 커야 합니다.");
                    x.RuleFor(x => x.DetailName)
                         .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("상세 키워드명은 필수입니다.");
                });
        }
    }

    public class UpdateSymptomExamKeywordCommandHandler : IRequestHandler<UpdateSymptomExamKeywordCommand, Result>
    {
        private readonly ILogger<UpdateSymptomExamKeywordCommandHandler> _logger;
        private readonly IHospitalManagementRepository _hospitalManagementRepository;
        private readonly IDbSessionRunner _db;

        public UpdateSymptomExamKeywordCommandHandler(
            ILogger<UpdateSymptomExamKeywordCommandHandler> logger, 
            IHospitalManagementRepository hospitalManagementRepository, 
            IDbSessionRunner db)
        {
            _logger = logger;
            _hospitalManagementRepository = hospitalManagementRepository;
            _db = db;
        }

        public async Task<Result> Handle(UpdateSymptomExamKeywordCommand req, CancellationToken ct)
        {
            _logger.LogInformation("Handling UpdateSymptomExamKeywordCommandHandler");

            var keywordMaster = req.Adapt<TbKeywordMasterEntity>();
            var keywordDetails = req.DetailKeywordItems.Adapt<List<TbKeywordDetailEntity>>();

            var result = await _db.RunAsync(DataSource.Hello100, 
                (session, token) => _hospitalManagementRepository.UpdateSymptomExamKeywordAsync(session, keywordMaster, keywordDetails, token),
            ct);

            return Result.Success();
        }
    }
}
