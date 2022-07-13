using FluentValidation;
using Infrastructure.Modules.Accounts.Requests;
using Infrastructure.Modules.Accounts.Services;

namespace Infrastructure.Modules.Accounts.Validations
{
    public class AccountDeleteValidation : AbstractValidator<AccountDeleteRequest>
    {
        private readonly IAccountService AccountService;
        public AccountDeleteValidation(IAccountService accountService)
        {
            AccountService = accountService;
            When(x => x.AccountIds is not null && x.AccountIds.Count != 0, () =>
            {
                RuleFor(x => x.AccountIds)
                .MustAsync(async (x, y, z) => { return await AccountService.IsAccountIdsExistsValidationAsync(x.AccountIds); }).WithMessage("{PropertyName}InValid")
                .MustAsync(async(x, y, z) => { return await AccountService.IsAccountIdsSameAsType(x.AccountIds); }).WithMessage("{PropertyName}MustSameAsType")
                .WithName("AccountIds");
            }).
           Otherwise(() =>
           {
               RuleFor(x => x.AccountIds)
               .NotNull().WithMessage("{PropertyName}IsRequired")
               .NotEmpty().WithMessage("{PropertyName}IsRequired")
               .WithName("AccountIds");
           });
        }
    }
}
