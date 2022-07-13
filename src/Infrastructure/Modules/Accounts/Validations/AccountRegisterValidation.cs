using FluentValidation;
using Infrastructure.Modules.Accounts.Requests;
using Infrastructure.Modules.Accounts.Services;
using Infrastructure.Modules.Applications.Services;
using System.Collections.Generic;

namespace Infrastructure.Modules.Accounts.Validations
{
    public class AccountRegisterValidation : AbstractValidator<AccountRegisterRequest>
    {
        private readonly IAccountService AccountService;
        private readonly IApplicationService ApplicationService;
        public AccountRegisterValidation(IAccountService accountService, IApplicationService applicationService)
        {
            AccountService = accountService;
            ApplicationService = applicationService;
            RuleFor(x => x.AccountFullName)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsRequired")
                .WithName("AccountFullName");
            RuleFor(x => x.AccountEmail)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsRequired")
                .EmailAddress().WithMessage("{PropertyName}MustBeEmail")
                .MustAsync(async (x, y, z) => { return await AccountService.IsEmailDoesNotExistValidationAsync(x.AccountEmail); }).WithMessage("{PropertyName}AlreadyExists")
                .WithName("AccountEmail");
            
            When(x => !string.IsNullOrEmpty(x.AccountIdentityCard), () =>
            {
                RuleFor(x => x.AccountIdentityCard)
               .Matches(@"[0-9]+").WithMessage("{PropertyName}MustIncludeNumericCharacters")
               .MaximumLength(12).WithMessage("{PropertyName}MustLessThanOrEqualTo12")
               .WithName("AccountIdentityCard");
            });
            When(x => x.AccountAvatar is not null, () =>
            {
                RuleFor(x => x.AccountAvatar)
                .Must((x, y) => { return AccountService.IsValidContentType(x.AccountAvatar, new List<string> { "image" }); }).WithMessage("{PropertyName}ContentTypeIsNotSupportedUpload")
                .WithName("AccountAvatar");
            });
            RuleFor(x => x.AccountPhone)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsRequired")
                .Matches(@"^[\+0-9]+$").WithMessage("{PropertyName}MustHaveNummericCharacterOr+Character")
                .MustAsync(async (x, y, z) => { return await AccountService.IsPhoneDoesNotExistValidationAsync(x.AccountPhone); }).WithMessage("{PropertyName}AlreadyExists")
                .WithName("AccountPhone");
            RuleFor(x => x.AccountUserName)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsRequired")
                .Matches(@".{8,}").WithMessage("{PropertyName}MustHaveLeast8Characters")
                .MaximumLength(255).WithMessage("{PropertyName}MustHaveLessThan255Characters")
                .MustAsync(async (x, y, z) => { return await AccountService.IsUserNameDoesNotExistsValidationAsync(x.AccountUserName); }).WithMessage("{PropertyName}AlreadyExists")
                .WithName("AccountUserName");
            RuleFor(x => x.AccountPassword)
              .NotNull().WithMessage("{PropertyName}IsRequired")
              .NotEmpty().WithMessage("{PropertyName}IsRequired")
              .Matches(@"[^a-zA-Z0-9]+").WithMessage("{PropertyName}MustIncludeSymbol")
              .Matches(@".{8,}").WithMessage("{PropertyName}MustHaveLeast8Characters")
              .Matches(@"[0-9]+").WithMessage("{PropertyName}MustIncludeNumericCharacters")
              .Matches(@"[A-Z]+").WithMessage("{PropertyName}MustIncludeUppercase")
              .Matches(@"[a-z]+").WithMessage("{PropertyName}MustIncludeLowercase")
              .WithName("AccountPassword");
            When(x => !string.IsNullOrEmpty(x.ApplicationId) && !string.IsNullOrEmpty(x.ApplicationSecret), () =>
            {
                RuleFor(x => x.ApplicationId)
                .MustAsync(async (x, y, z) => { return await ApplicationService.IsApplicationIdValidValidationAsync(x.ApplicationId); }).WithMessage("{PropertyName}InValid")
                .MustAsync(async(x, y, z) => { return await ApplicationService.IsApplicationActiveValidationAsync(x.ApplicationId); }).WithMessage("{PropertyName}IsLocked")
                .WithName("ApplicationId");

                RuleFor(x => x.ApplicationSecret)
               .MustAsync(async (x, y, z) => { return await AccountService.IsApplicationSecretValidValidationAsync(x.ApplicationId, x.ApplicationSecret); }).WithMessage("{PropertyName}InValid")
               .WithName("ApplicationSecret");

            });
        }
    }
}
