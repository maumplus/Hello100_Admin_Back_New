using FluentValidation;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.CreateSeller
{
    /// <summary>
    /// 송금 등록 요청 데이터 검증
    /// </summary>
    public class CreateSellerCommandValidator : AbstractValidator<CreateSellerCommand>
    {
        public CreateSellerCommandValidator()
        {
            RuleFor(x => x.HospNo)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("요양기관번호는 필수입니다.");

            RuleFor(x => x.BankCd)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("은행코드는 필수입니다.");

            RuleFor(x => x.DepositNo)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("계좌번호는 필수입니다.");

            RuleFor(x => x.Depositor)
                .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("예금주명은 필수입니다.");
        }
    }
}
