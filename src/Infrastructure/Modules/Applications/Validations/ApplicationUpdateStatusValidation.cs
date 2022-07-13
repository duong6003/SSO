using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Infrastructure.Modules.Applications.Requests;
using Infrastructure.Modules.Applications.Services;
using System.Linq;

namespace Infrastructure.Modules.Applications.Validations
{
    public class ApplicationUpdateStatusValidation : AbstractValidator<ApplicationUpdateStatusRequest>
    {
        private readonly IApplicationService ApplicationService;
        public ApplicationUpdateStatusValidation(IApplicationService applicationService, IActionContextAccessor actionContextAccessor)
        {
            ApplicationService = applicationService;
            string applicationId = (string)actionContextAccessor.ActionContext.RouteData.Values
                .Where(o => o.Key == "applicationId")
                .Select(o => o.Value)
                .FirstOrDefault();
            RuleFor(x => x)
                .Cascade(CascadeMode.Stop)
                .MustAsync(async (x, y) => { return await ApplicationService.IsApplicationIdValidValidationAsync(applicationId); }).WithMessage("{PropertyName}NotFound")
                .WithName("applicationId");
            RuleFor(x => x.ApplicationStatus)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .IsInEnum().WithMessage("{PropertyName}InValid")
                .WithName("ApplicationStatus");
        }
    }
}
