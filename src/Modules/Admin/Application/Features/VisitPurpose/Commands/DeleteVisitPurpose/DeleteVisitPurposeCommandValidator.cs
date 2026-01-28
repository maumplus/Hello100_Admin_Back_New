using FluentValidation;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.DeleteVisitPurpose
{
    public class DeleteVisitPurposeCommandValidator : AbstractValidator<DeleteVisitPurposeCommand>
    {
        public DeleteVisitPurposeCommandValidator()
        {
            RuleFor(x => x.VpCd)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("내원 키는 필수 항목입니다.");
            RuleFor(x => x.HospKey)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("요양기관 키는 필수 항목입니다.");
        }
    }
}
