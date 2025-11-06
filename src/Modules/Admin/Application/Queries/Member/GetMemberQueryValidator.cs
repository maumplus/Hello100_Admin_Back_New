using FluentValidation;

namespace Hello100Admin.Modules.Admin.Application.Queries.Member;

/// <summary>
/// GetMemberQuery Validator
/// </summary>
public class GetMemberQueryValidator : AbstractValidator<GetMemberQuery>
{
    public GetMemberQueryValidator()
    {
        RuleFor(x => x.Uid)
            .NotEmpty().WithMessage("고객 ID는 필수입니다.")
            .MinimumLength(5).WithMessage("고객 ID는 최소 5자 이상이어야 합니다.");
    }
}
