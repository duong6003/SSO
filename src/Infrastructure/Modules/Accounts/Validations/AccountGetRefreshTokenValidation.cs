using FluentValidation;
using Infrastructure.Modules.Accounts.Requests;

namespace Infrastructure.Modules.Accounts.Validations
{
    public class AccountGetRefreshTokenValidation : AbstractValidator<AccountGetRefreshTokenRequest>
    {
        public AccountGetRefreshTokenValidation()
        {
            RuleFor(x => x.OldToken)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsRequired")
                .WithName("OldToken");
            RuleFor(x => x.RefreshToken)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsRequired")
                .WithName("RefreshToken");
        }
    }
}
