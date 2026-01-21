using FluentValidation;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.GetRegistrationAlimtalkApplicationInfo
{
    public class GetRegistrationAlimtalkApplicationInfoQueryValidator : AbstractValidator<GetRegistrationAlimtalkApplicationInfoQuery>
    {
        public GetRegistrationAlimtalkApplicationInfoQueryValidator()
        {
            RuleFor(x => x.HospNo).NotEmpty().WithMessage("병원 번호는 필수입니다.");
        }
    }
}
