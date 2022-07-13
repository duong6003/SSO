using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Infrastructure.Modules.Accounts.Requests;
using Infrastructure.Modules.Accounts.Services;
using Infrastructure.Modules.Applications.Services;
using System.Linq;

namespace Infrastructure.Modules.Accounts.Validations
{
    public class AccountUpdateTwoFactorAuthValidation : AbstractValidator<AccountUpdateTwoFactorRequest>
    {
        public AccountUpdateTwoFactorAuthValidation(IAccountService accountService, IActionContextAccessor actionContextAccessor, IApplicationService applicationService)
        {
            string accountUserName = (string)actionContextAccessor.ActionContext.RouteData.Values
                .Where(o => o.Key == "accountUserName")
                .Select(o => o.Value)
                .FirstOrDefault();
            RuleFor(x => x)
                .Cascade(CascadeMode.Stop)
                .MustAsync(async (x, y, z) => { return await accountService.IsAccountUserNameExistsValidationAsync(accountUserName); }).WithMessage("{PropertyName}NotFound")
                .WithName("AccountUserName");
            RuleFor(x => x.AccountTwoFactorAuth)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .IsInEnum().WithMessage("{PropertyName}InValid")
                .WithName("AccountTwoFactorAuth");
            When(x => !string.IsNullOrEmpty(x.ApplicationId) && !string.IsNullOrEmpty(x.ApplicationSecret), () =>
            {
                RuleFor(x => x.ApplicationId)
                .MustAsync(async (x, y, z) => { return await applicationService.IsApplicationIdValidValidationAsync(x.ApplicationId); }).WithMessage("{PropertyName}InValid")
                .MustAsync(async (x, y, z) => { return await applicationService.IsApplicationActiveValidationAsync(x.ApplicationId); }).WithMessage("{PropertyName}IsLocked")
                .WithName("ApplicationId");

                RuleFor(x => x.ApplicationSecret)
               .MustAsync(async (x, y, z) => { return await accountService.IsApplicationSecretValidValidationAsync(x.ApplicationId, x.ApplicationSecret); }).WithMessage("{PropertyName}InValid")
               .WithName("ApplicationSecret");

            }).Otherwise(() =>
            {
                RuleFor(x => x.ApplicationId)
                .Must((x, y) => !string.IsNullOrEmpty(x.ApplicationId)).WithMessage("{PropertyName}IsRequired")
                .WithName("ApplicationId");

                RuleFor(x => x.ApplicationSecret)
                .Must((x, y) => !string.IsNullOrEmpty(x.ApplicationSecret)).WithMessage("{PropertyName}IsRequired")
               .WithName("ApplicationSecret");
            });
        }
    }
}
