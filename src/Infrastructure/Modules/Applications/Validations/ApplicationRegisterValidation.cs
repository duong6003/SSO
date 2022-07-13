using FluentValidation;
using Infrastructure.Modules.Accounts.Services;
using Infrastructure.Modules.Applications.Requests;
using Infrastructure.Modules.Applications.Services;
using System.Collections.Generic;

namespace Infrastructure.Modules.Applications.Validations
{
    public class ApplicationRegisterValidation : AbstractValidator<ApplicationRegisterRequest>
    {
        private readonly IApplicationService ApplicationService;

        public ApplicationRegisterValidation(IApplicationService applicationService, IAccountService accountService)
        {
            ApplicationService = applicationService;
            RuleFor(x => x.ApplicationName)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsRequired")
                .WithName("ApplicationName");
            RuleFor(x => x.ApplicationRedirectUrl)
               .NotNull().WithMessage("{PropertyName}IsRequired")
               .NotEmpty().WithMessage("{PropertyName}IsRequired")
               .Must((x, y) => { return ApplicationService.IsUrlValidValidation(x.ApplicationRedirectUrl); }).WithMessage("{PropertyName}MustBeValidUrl")
               .WithName("RedirectUrl");
            RuleFor(x => x.ApplicationWebHook)
               .NotNull().WithMessage("{PropertyName}IsRequired")
               .NotEmpty().WithMessage("{PropertyName}IsRequired")
               .Must((x, y) => { return ApplicationService.IsUrlValidValidation(x.ApplicationWebHook); }).WithMessage("{PropertyName}MustBeValidUrl")
               .WithName("ApplicationWebHook");

            When(x => x.ApplicationIcon is not null, () =>
            {
                RuleFor(x => x.ApplicationIcon)
                    .Must((x, y) => { return accountService.IsValidContentType(x.ApplicationIcon, new List<string> { "image" }); }).WithMessage("{PropertyName}ContentTypeIsNotSupportedUpload");
            });
        }


    }
}
