using FluentValidation;
using Infrastructure.Modules.Accounts.Requests;
using Infrastructure.Modules.Accounts.Services;

namespace Infrastructure.Modules.Accounts.Validations
{
    public class AccountVerifyOTPValidation : AbstractValidator<AccountVeriryOTPRequest>
    {
        private readonly IAccountService AccountService;

        public AccountVerifyOTPValidation(IAccountService accountService)
        {
            AccountService = accountService;
            RuleFor(x => x.AccountId)
                .Cascade(CascadeMode.Stop)
                .MustAsync(async (x, y, z) => await AccountService.GetByIdAsync(x.AccountId) is not null).WithMessage("{PropertyName}InValid")
                .WithName("AccountId");
            RuleFor(x => x.OTPCode)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsRequired")
                .Must(x => x.Length == 6).WithMessage("{PropertyName}MustEqual6Character")
                .Must(x => int.TryParse(x, out int value)).WithMessage("{PropertyName}MustNumeric")
                .MustAsync(async (x, y, z) => { return await AccountService.IsCorrectOTPCodeValidationAsync(x.AccountId, x.OTPCode); }).WithMessage("{PropertyName}InCorrectOrExpired")
                .MustAsync(async (x, y, z) => { return await AccountService.IsCorrectTryNumberInputOTPAsync(x.AccountId, x.OTPCode); }).WithMessage("{PropertyName}MoreThan5TimesAllowed")
                .WithName("OTPCode");
        }
    }
}
