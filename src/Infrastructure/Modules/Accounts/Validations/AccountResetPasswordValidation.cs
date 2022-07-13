using FluentValidation;
using Infrastructure.Modules.Accounts.Requests;
using Infrastructure.Modules.Accounts.Services;

namespace Infrastructure.Modules.Accounts.Validations
{
    public class AccountResetPasswordValidation : AbstractValidator<AccountResetPasswordRequest>
    {
        private readonly IAccountService AccountService;

        public AccountResetPasswordValidation(IAccountService accountService)
        {
            AccountService = accountService;
            RuleFor(x => x.AccountEmail)
               .NotNull().WithMessage("{PropertyName}IsRequired")
               .NotEmpty().WithMessage("{PropertyName}IsRequired")
               .EmailAddress().WithMessage("{PropertyName}MustBeEmail")
               .MustAsync(async (x, y, z) => { return await AccountService.IsEmailExistValidationAsync(x.AccountEmail); }).WithMessage("{PropertyName}DoesNotExists")
               .WithName("AccountEmail");
        }
    }
}
