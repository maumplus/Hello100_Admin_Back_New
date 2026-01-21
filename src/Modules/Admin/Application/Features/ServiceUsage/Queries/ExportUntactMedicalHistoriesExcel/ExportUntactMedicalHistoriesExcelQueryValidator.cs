using FluentValidation;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.ExportUntactMedicalHistoriesExcel
{
    public class ExportUntactMedicalHistoriesExcelQueryValidator : AbstractValidator<ExportUntactMedicalHistoriesExcelQuery>
    {
        public ExportUntactMedicalHistoriesExcelQueryValidator()
        {
            RuleFor(x => x.SearchDateType).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("날짜 기준 선택은 필수입니다.");
        }
    }
}
