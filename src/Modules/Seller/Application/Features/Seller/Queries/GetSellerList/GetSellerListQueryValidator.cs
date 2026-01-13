using FluentValidation;

namespace Hello100Admin.Modules.Seller.Application.Features.Seller.Queries.GetSellerList
{
    public class GetSellerListQueryValidator : AbstractValidator<GetSellerListQuery>
    {
        public GetSellerListQueryValidator()
        {
            RuleFor(x => x.PageNo).NotNull().GreaterThan(0).WithMessage("페이지 번호는 필수이며 0보다 커야 합니다.");
            RuleFor(x => x.PageSize).NotNull().GreaterThan(0).WithMessage("페이지 사이즈는 필수이며 0보다 커야 합니다.");
        }
    }
}
