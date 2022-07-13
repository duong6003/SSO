using FluentValidation;
using Infrastructure.Modules.Accounts.Requests;
using Infrastructure.Modules.Accounts.Services;
using Infrastructure.Modules.Applications.Services;

namespace Infrastructure.Modules.Accounts.Validations
{
    public class GetTokenApplicationValidation : AbstractValidator<GetTokenApplicationRequest>
    {
        private readonly IApplicationService ApplicationService;
        private readonly IAccountService AccountService;
        public GetTokenApplicationValidation(IApplicationService applicationService, IAccountService accountService)
        {
            ApplicationService = applicationService;
            AccountService = accountService;
            RuleFor(x => x.Client_id)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsRequired")
                .MustAsync(async (x, y, z) => { return await ApplicationService.IsApplicationIdValidValidationAsync(x.Client_id); }).WithMessage("{PropertyName}InValid")
                .MustAsync(async(x, y, z) => { return await ApplicationService.IsApplicationActiveValidationAsync(x.Client_id); }).WithMessage("{PropertyName}IsLocked")
                .WithName("Client_id");
            RuleFor(x => x.Client_secret)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsRequired")
                .MustAsync(async (x, y, z) => { return await ApplicationService.IsApplicationSecretValidValidationAsync(x.Client_id, x.Client_secret); }).WithMessage("{PropertyName}InValid")
                .WithName("Client_secret");
           
        }
    }
}
