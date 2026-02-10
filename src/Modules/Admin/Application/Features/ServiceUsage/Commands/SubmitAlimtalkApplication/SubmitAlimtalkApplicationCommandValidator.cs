using FluentValidation;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Commands.SubmitAlimtalkApplication
{
    public class SubmitAlimtalkApplicationCommandValidator : AbstractValidator<SubmitAlimtalkApplicationCommand>
    {
        public SubmitAlimtalkApplicationCommandValidator()
        {
            RuleFor(x => x.HospNo)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("요양기관번호가 비어있습니다. 다시 로그인해주세요.");
            RuleFor(x => x.HospKey)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("요양기관 키가 비어있습니다. 다시 로그인해주세요.");
            RuleFor(x => x.DoctNm)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("신청인을 입력해주세요.")
                .MaximumLength(50).WithMessage("신청인은 최대 50자까지 입력 가능합니다.");
            RuleFor(x => x.DoctTel)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("휴대 전화번호를 입력해주세요.")
                .MaximumLength(20).WithMessage("휴대 전화번호는 최대 20자까지 입력 가능합니다.");
        }
    }
}
