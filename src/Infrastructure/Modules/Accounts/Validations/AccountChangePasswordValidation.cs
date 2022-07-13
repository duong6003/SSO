using FluentValidation;
using Infrastructure.Modules.Accounts.Requests;
using Infrastructure.Modules.Accounts.Services;

namespace Infrastructure.Modules.Accounts.Validations
{
    public class AccountChangePasswordValidation : AbstractValidator<AccountChangePasswordRequest>
    {
        
        public AccountChangePasswordValidation()
        {
            RuleFor(x => x.NewPassword)
               .Matches(@"[^a-zA-Z0-9]+").WithMessage("{PropertyName}MustIncludeSymbol")
               .Matches(@".{8,}").WithMessage("{PropertyName}MustHaveLeast8Characters")
               .Matches(@"[0-9]+").WithMessage("{PropertyName}MustIncludeNumericCharacters")
               .Matches(@"[A-Z]+").WithMessage("{PropertyName}MustIncludeUppercase")
               .Matches(@"[a-z]+").WithMessage("{PropertyName}MustIncludeLowercase")
               .WithName("UserPassword");
        }
    }
}
