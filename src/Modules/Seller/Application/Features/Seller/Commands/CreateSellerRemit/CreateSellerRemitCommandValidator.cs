using FluentValidation;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.CreateSellerRemit
{
    public class CreateSellerRemitCommandValidator : AbstractValidator<CreateSellerRemitCommand>
    {
        public CreateSellerRemitCommandValidator()
        {
            RuleFor(x => x.HospSellerId).NotNull().GreaterThan(0).WithMessage("판매자 일련번호는 필수이며 0보다 커야 합니다.");
            //RuleFor(x => x.Amount).GreaterThan(0).WithMessage("송금 금액은 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.Amount).NotNull().InclusiveBetween(1000, int.MaxValue).WithMessage("송금 금액은 필수이며 1000 이상이어야 합니다."); // 1,000 ~ 2,147,483,647
            RuleFor(x => x.Etc).MaximumLength(500).WithMessage("기타 정보는 최대 500자까지 입력할 수 있습니다.");
        }
    }
}
