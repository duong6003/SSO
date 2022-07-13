using FluentValidation;
using Infrastructure.Modules.Accounts.Requests;
using Infrastructure.Modules.Accounts.Services;
using Infrastructure.Modules.Applications.Services;

namespace Infrastructure.Modules.Accounts.Validations
{
    public class GetProfileAccountIdApplicationValidation : AbstractValidator<GetProfileAccountIdApplicationRequest>
    {
        private readonly IAccountService AccountService;
        private readonly IApplicationService ApplicationService;

        public GetProfileAccountIdApplicationValidation(IAccountService accountService, IApplicationService applicationService)
        {
            AccountService = accountService;
            ApplicationService = applicationService;
            RuleFor(x => x.ApplicationId)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsRequired")
                .MustAsync(async (x, y, z) => { return await ApplicationService.IsApplicationIdValidValidationAsync(x.ApplicationId); }).WithMessage("{PropertyName}InValid")
                .MustAsync(async (x, y, z) => { return await ApplicationService.IsApplicationActiveValidationAsync(x.ApplicationId); }).WithMessage("{PropertyName}IsLocked")
                .WithName("ApplicationId");
            RuleFor(x => x.ApplicationSecret)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsRequired")
                .MustAsync(async (x, y, z) => { return await ApplicationService.IsApplicationSecretValidValidationAsync(x.ApplicationId, x.ApplicationSecret); }).WithMessage("{PropertyName}InValid")
                .WithName("ApplicationSecret");
            When(x => !string.IsNullOrEmpty(x.AccountId), () =>
            {
                RuleFor(x => x.AccountId)
                    .MustAsync(async (x, y, z) => { return await AccountService.GetByIdAsync(x.AccountId) is not null; }).WithMessage("{PropertyName}InValid")
                    .WithName("AccountId");
            });
        }
    }
}
