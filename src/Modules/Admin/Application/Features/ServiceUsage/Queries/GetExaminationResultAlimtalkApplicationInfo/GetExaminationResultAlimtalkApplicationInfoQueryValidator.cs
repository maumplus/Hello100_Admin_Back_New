using FluentValidation;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.GetExaminationResultAlimtalkApplicationInfo
{
    public class GetExaminationResultAlimtalkApplicationInfoQueryValidator : AbstractValidator<GetExaminationResultAlimtalkApplicationInfoQuery>
    {
        public GetExaminationResultAlimtalkApplicationInfoQueryValidator()
        {
            RuleFor(x => x.HospNo).NotEmpty().WithMessage("병원 번호는 필수입니다.");
        }
    }
}
