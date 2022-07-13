using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Infrastructure.Modules.Accounts.Services;
using Infrastructure.Modules.Applications.Requests;
using Infrastructure.Modules.Applications.Services;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Modules.Applications.Validations
{
    public class ApplicationUpdateValidation : AbstractValidator<ApplicationUpdateRequest>
    {
        private readonly IApplicationService ApplicationService;

        public ApplicationUpdateValidation(IApplicationService applicationService, IActionContextAccessor actionContextAccessor, IAccountService accountService)
        {
            ApplicationService = applicationService;
            string applicationId = (string)actionContextAccessor.ActionContext.RouteData.Values
                .Where(o => o.Key == "applicationId")
                .Select(o => o.Value)
                .FirstOrDefault();
            RuleFor(x => x)
                .Cascade(CascadeMode.Stop)
                .MustAsync(async (x, y) => { return await ApplicationService.IsApplicationIdValidValidationAsync(applicationId); }).WithMessage("{PropertyName}InValid")
                .WithName("applicationId");
            When(x => !string.IsNullOrEmpty(x.ApplicationRedirectUrl), () =>
            {
                RuleFor(x => x.ApplicationRedirectUrl)
               .Must((x, y) => { return ApplicationService.IsUrlValidValidation(x.ApplicationRedirectUrl); }).WithMessage("{PropertyName}MustBeValidUrl")
               .WithName("ApplicationRedirectUrl");
            });
            When(x => !string.IsNullOrEmpty(x.ApplicationWebHook), () =>
            {
                RuleFor(x => x.ApplicationWebHook)
               .Must((x, y) => { return ApplicationService.IsUrlValidValidation(x.ApplicationWebHook); }).WithMessage("{PropertyName}MustBeValidUrl")
               .WithName("ApplicationWebHook");
            });
            When(x => x.ApplicationIcon is not null, () =>
            {
                RuleFor(x => x.ApplicationIcon)
                    .Must((x, y) => { return accountService.IsValidContentType(x.ApplicationIcon, new List<string> { "image" }); }).WithMessage("{PropertyName}ContentTypeIsNotSupportedUpload");
            });
        }
    }
}
