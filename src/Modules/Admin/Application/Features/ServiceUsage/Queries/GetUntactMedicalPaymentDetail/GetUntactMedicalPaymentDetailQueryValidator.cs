using FluentValidation;

namespace Hello100Admin.Modules.Admin.Application.Features.ServiceUsage.Queries.GetUntactMedicalPaymentDetail
{
    public class GetUntactMedicalPaymentDetailQueryValidator : AbstractValidator<GetUntactMedicalPaymentDetailQuery>
    {
        public GetUntactMedicalPaymentDetailQueryValidator()
        {
            RuleFor(x => x.PaymentId)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("NHN KCP 결제 ID는 필수입니다.");
        }
    }
}
