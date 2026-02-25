using FluentValidation;

namespace Hello100Admin.Modules.Admin.Application.Features.VisitPurpose.Commands.BulkUpdateVisitPurposes
{
    public class BulkUpdateVisitPurposesCommandValidator : AbstractValidator<BulkUpdateVisitPurposesCommand>
    {
        public BulkUpdateVisitPurposesCommandValidator()
        {
            RuleFor(x => x.HospKey)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("요양기관 키는 필수입니다.");
        }
    }
}
