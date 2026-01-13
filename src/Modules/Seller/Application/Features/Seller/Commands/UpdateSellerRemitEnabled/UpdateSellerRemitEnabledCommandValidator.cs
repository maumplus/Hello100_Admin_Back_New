using FluentValidation;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Commands.UpdateSellerRemitEnabled
{
    public class UpdateSellerRemitEnabledCommandValidator : AbstractValidator<UpdateSellerRemitEnabledCommand>
    {
        public UpdateSellerRemitEnabledCommandValidator()
        {
            RuleFor(x => x.Id).NotNull().GreaterThan(0).WithMessage("Id는 0보다 커야 합니다.");
        }
    }
}
