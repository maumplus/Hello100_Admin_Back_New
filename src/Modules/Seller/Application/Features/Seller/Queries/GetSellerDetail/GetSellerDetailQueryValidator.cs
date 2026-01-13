using FluentValidation;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetSellerDetail
{
    public class GetSellerDetailQueryValidator : AbstractValidator<GetSellerDetailQuery>
    {
        public GetSellerDetailQueryValidator()
        {
            RuleFor(x => x.Id).NotNull().GreaterThan(0).WithMessage("ID 값은 필수이며 0보다 커야 합니다.");
        }
    }
}
