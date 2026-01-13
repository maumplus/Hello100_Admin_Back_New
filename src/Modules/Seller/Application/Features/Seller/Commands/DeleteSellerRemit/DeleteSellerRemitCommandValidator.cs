using FluentValidation;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.DeleteSellerRemit
{
    public class DeleteSellerRemitCommandValidator : AbstractValidator<DeleteSellerRemitCommand>
    {
        public DeleteSellerRemitCommandValidator()
        {
            RuleFor(x => x.Id).NotNull().GreaterThan(0).WithMessage("송금 요청 일련번호는 필수이며 0보다 커야 합니다.");
        }
    }
}
