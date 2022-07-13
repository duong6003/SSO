using FluentValidation;
using Infrastructure.Modules.Accounts.Requests;
using Infrastructure.Modules.Accounts.Services;
using System.Collections.Generic;

namespace Infrastructure.Modules.Accounts.Validations
{
    public class AccountStoreValidation : AbstractValidator<AccountStoreRequest>
    {
        private readonly IAccountService AccountService;

        public AccountStoreValidation(IAccountService accountService)
        {
            AccountService = accountService;
            RuleFor(x => x.AccountFullName)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsRequired")
                .WithName("AccountFullName");
            RuleFor(x => x.AccountEmail)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsRequired")
                .EmailAddress().WithMessage("{PropertyName}:MustBeEmail")
                .MustAsync(async (x, y, z) => { return await AccountService.IsEmailDoesNotExistValidationAsync(x.AccountEmail); }).WithMessage("{PropertyName}AlreadyExists")
                .WithName("AccountEmail");
            RuleFor(x => x.AccountPhone)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsRequired")
                .Matches(@"^[\+0-9]+$").WithMessage("{PropertyName}MustHaveNummericCharacterOr+Character")
                .MustAsync(async (x, y, z) => { return await AccountService.IsPhoneDoesNotExistValidationAsync(x.AccountPhone); }).WithMessage("{PropertyName}AlreadyExists")
                .WithName("AccountPhone");
            RuleFor(x => x.AccountUserName)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsRequired")
                .MaximumLength(12).WithMessage("{PropertyName}MustLessThanOrEqualTo12")
                .Matches(@".{8,}").WithMessage("{PropertyName}MustHaveLeast8Characters")
                .MaximumLength(255).WithMessage("{PropertyName}MustHaveLessThan255Characters")
                .MustAsync(async (x, y, z) => { return await AccountService.IsUserNameDoesNotExistsValidationAsync(x.AccountUserName); }).WithMessage("{PropertyName}AlreadyExists")
                .WithName("AccountUserName");
            When(x => !string.IsNullOrEmpty(x.AccountIdentityCard), () =>
            {
                RuleFor(x => x.AccountIdentityCard)
               .Matches(@"[0-9]+").WithMessage("{PropertyName}MustIncludeNumericCharacters")
               .WithName("AccountIdentityCard");
            });
            When(x => x.AccountAvatar is not null, () =>
            {
                RuleFor(x => x.AccountAvatar)
                .Must((x, y) => { return AccountService.IsValidContentType(x.AccountAvatar, new List<string> { "image" }); }).WithMessage("{PropertyName}ContentTypeIsNotSupportedUpload")
                .WithName("AccountAvatar");
            });
            RuleFor(x => x.AccountPassword)
              .NotNull().WithMessage("{PropertyName}IsRequired")
              .NotEmpty().WithMessage("{PropertyName}IsRequired")
              .Matches(@"[^a-zA-Z0-9]+").WithMessage("{PropertyName}MustIncludeSymbol")
              .Matches(@".{8,}").WithMessage("{PropertyName}MustHaveLeast8Characters")
              .Matches(@"[0-9]+").WithMessage("{PropertyName}MustIncludeNumericCharacters")
              .Matches(@"[A-Z]+").WithMessage("{PropertyName}MustIncludeUppercase")
              .Matches(@"[a-z]+").WithMessage("{PropertyName}MustIncludeLowercase")
              .WithName("AccountPassword");

            RuleFor(x => x.AccountTwoFactorAuth)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .IsInEnum().WithMessage("{PropertyName}InValid")
                .WithName("AccountTwoFactorAuth");
            When(x => x.PermissionCodes is not null && x.PermissionCodes.Count != 0, () =>
            {
                RuleFor(x => x.PermissionCodes)
                .MustAsync(async (x, y, z) => { return await accountService.IsPermissionCodesExistsValidationAsync(x.PermissionCodes); }).WithMessage("{PropertyName}Invalid")
                .WithName("PermissionCodes");
            }).Otherwise(() =>
            {
                RuleFor(x => x.PermissionCodes)
                .NotNull().WithMessage("{PropertyName}IsRequired")
                .NotEmpty().WithMessage("{PropertyName}IsRequired")
                .WithName("PermissionCodes");
            });
        }
    }
}
