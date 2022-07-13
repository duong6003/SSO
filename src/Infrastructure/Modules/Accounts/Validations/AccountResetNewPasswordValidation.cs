using FluentValidation;
using Infrastructure.Modules.Accounts.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Modules.Accounts.Validations
{
    public class AccountResetNewPasswordValidation : AbstractValidator<AccountResetNewPasswordRequest>
    {
        public AccountResetNewPasswordValidation()
        {
            RuleFor(x => x.AccountNewPassword)
               .Must((x, y) => !string.IsNullOrEmpty(x.AccountNewPassword)).WithMessage("{PropertyName}IsRequired")
               .Matches(@"[^a-zA-Z0-9]+").WithMessage("{PropertyName}MustIncludeSymbol")
               .Matches(@".{8,}").WithMessage("{PropertyName}MustHaveLeast8Characters")
               .Matches(@"[0-9]+").WithMessage("{PropertyName}MustIncludeNumericCharacters")
               .Matches(@"[A-Z]+").WithMessage("{PropertyName}MustIncludeUppercase")
               .Matches(@"[a-z]+").WithMessage("{PropertyName}MustIncludeLowercase")
               .WithName("AccountNewPasword");
            RuleFor(x => x.RecoveryToken)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .WithName("RecoveryToken");
                
        }
    }
}
