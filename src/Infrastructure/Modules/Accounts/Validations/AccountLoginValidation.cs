using FluentValidation;
using Infrastructure.Modules.Accounts.Requests;
using Infrastructure.Modules.Accounts.Services;
using Infrastructure.Modules.Applications.Services;

namespace Infrastructure.Modules.Accounts.Validations
{
    public class AccountLoginValidation : AbstractValidator<AccountLoginRequest>
    {
        private readonly IAccountService AccountService;
        private readonly IApplicationService ApplicationService;
        public AccountLoginValidation(IAccountService accountService, IApplicationService applicationService)
        {
            AccountService = accountService;
            ApplicationService = applicationService;
            CascadeMode = CascadeMode.Stop;
            RuleFor(x => x.AccountUserName)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsRequired")
                .MustAsync(async (x, y, z) => { return await AccountService.IsAccountUserNameExistsValidationAsync(x.AccountUserName); }).WithMessage("{PropertyName}DoesNotExists")
                .MustAsync(async (x, y, z) => { return await AccountService.IsAccountUserNameActiveValidationAsync(x.AccountUserName); }).WithMessage("{PropertyName}IsLocked")
                .WithName("AccountUserName");
            RuleFor(x => x.AccountPassword)
              .NotNull().WithMessage("{PropertyName}IsRequired")
              .NotEmpty().WithMessage("{PropertyName}IsRequired")
              //.MustAsync(async (x, y, z) => { return await AccountService.IsPasswordCorrectValidationAsync(x.AccountUserName, x.AccountPassword); }).WithMessage("{PropertyName}InCorrect")
              .WithName("AccountPassword");

            RuleFor(x => x.ReCaptcha)
              .NotNull().WithMessage("{PropertyName}IsRequired")
              .NotEmpty().WithMessage("{PropertyName}IsRequired")
              .Must((x, y) => { return accountService.VerifyCaptchaAsync(x.ReCaptcha); }).WithMessage("{PropertyName}InValid")
              .WithName("ReCaptcha");


            When(x => !string.IsNullOrEmpty(x.ApplicationId), () =>
            {
                RuleFor(x => x.ApplicationId)
                    .MustAsync(async (x, y, z) => { return await ApplicationService.IsApplicationIdValidValidationAsync(x.ApplicationId); }).WithMessage("{PropertyName}InValid")
                    .MustAsync(async (x, y, z) => { return await ApplicationService.IsApplicationActiveValidationAsync(x.ApplicationId); }).WithMessage("{PropertyName}IsLocked")
                    .WithName("ApplicationId");
            });
            When(x => !string.IsNullOrEmpty(x.SuccessUrl), () =>
            {
                RuleFor(x => x.SuccessUrl)
                    .Must((x, y) => { return ApplicationService.IsUrlValidValidation(x.SuccessUrl); }).WithMessage("{PropertyName}MustBeValidUrl")
                    .WithName("SuccessUrl");
            });
        }
    }
}
