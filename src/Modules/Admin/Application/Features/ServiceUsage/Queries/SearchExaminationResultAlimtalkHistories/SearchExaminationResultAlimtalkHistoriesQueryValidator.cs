using FluentValidation;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.SearchExaminationResultAlimtalkHistories
{
    public class SearchExaminationResultAlimtalkHistoriesQueryValidator : AbstractValidator<SearchExaminationResultAlimtalkHistoriesQuery>
    {
        public SearchExaminationResultAlimtalkHistoriesQueryValidator()
        {
            RuleFor(x => x.PageNo).NotNull().GreaterThan(0).WithMessage("페이지 번호는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.PageSize).NotNull().GreaterThan(0).WithMessage("페이지 사이즈는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.FromDate).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("조회 시작일 선택은 필수입니다.");
            RuleFor(x => x.ToDate).Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("조회 종료일 선택은 필수입니다.");
            RuleFor(x => x.SearchDateType).NotNull().GreaterThan(0).WithMessage("날짜 기준 선택은 필수이며, 0보다 커야 합니다.");
            RuleFor(x => x.SendStatus).NotNull().GreaterThanOrEqualTo(0).WithMessage("발송 상태 선택은 필수이며, 0이상이어야 합니다.");
        }
    }
}
