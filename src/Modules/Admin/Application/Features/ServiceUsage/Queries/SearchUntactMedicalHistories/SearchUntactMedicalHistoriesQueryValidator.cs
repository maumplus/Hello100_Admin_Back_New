using FluentValidation;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.SearchUntactMedicalHistories
{
    public class SearchUntactMedicalHistoriesQueryValidator : AbstractValidator<SearchUntactMedicalHistoriesQuery>
    {
        public SearchUntactMedicalHistoriesQueryValidator()
        {
            RuleFor(x => x.PageNo).NotNull().GreaterThan(0).WithMessage("페이지 번호는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.PageSize).NotNull().GreaterThan(0).WithMessage("페이지 사이즈는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.SearchDateType).NotNull().WithMessage("날짜 기준 선택은 필수입니다.");
        }
    }
}
