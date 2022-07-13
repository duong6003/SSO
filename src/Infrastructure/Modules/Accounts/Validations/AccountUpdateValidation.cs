using FluentValidation;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Infrastructure.Modules.Accounts.Requests;
using Infrastructure.Modules.Accounts.Services;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Modules.Accounts.Validations
{
    public class AccountUpdateValidation : AbstractValidator<AccountUpdateRequest>
    {
        private readonly IAccountService AccountService;
        public AccountUpdateValidation(IAccountService accountService, IActionContextAccessor actionContextAccessor)
        {
            AccountService = accountService;
            string accountId = (string)actionContextAccessor.ActionContext.RouteData.Values
                .Where(o => o.Key == "accountId")
                .Select(o => o.Value)
                .FirstOrDefault();
            RuleFor(x => x)
                .Cascade(CascadeMode.Stop)
                .MustAsync(async (x, y, z) => await accountService.GetByIdAsync(accountId) is not null).WithMessage("{PropertyName}NotFound")
                .WithName("accountId");
            When(x => !string.IsNullOrEmpty(x.AccountEmail), () =>
            {
                RuleFor(x => x.AccountEmail)
                    .EmailAddress().WithMessage("{PropertyName}MustBeEmail")
                    .MustAsync(async (x, y, z) => { return await AccountService.IsEmailDoesNotExistValidationAsync(x.AccountEmail, accountId); }).WithMessage("{PropertyName}AlreadyExists")
                    .WithName("AccountEmail");
            });
            When(x => !string.IsNullOrEmpty(x.AccountPhone), () =>
            {
                RuleFor(x => x.AccountPhone)
                    .MustAsync(async (x, y, z) => { return await AccountService.IsPhoneDoesNotExistValidationAsync(x.AccountPhone, accountId); }).WithMessage("{PropertyName}AlreadyExists")
                    .Matches(@"^[\+0-9]+$").WithMessage("{PropertyName}MustHaveNummericCharacterOr+Character")
                    .WithName("AccountPhone");
            });
            When(x => !string.IsNullOrEmpty(x.AccountIdentityCard), () =>
            {
                RuleFor(x => x.AccountIdentityCard)
               .Matches(@"[0-9]+").WithMessage("{PropertyName}MustIncludeNumericCharacters")
               .MaximumLength(12).WithMessage("{PropertyName}MustLessThanOrEqualTo12")
               .WithName("AccountIdentityCard");
            });
            When(x => !string.IsNullOrEmpty(x.AccountUserName), () =>
            {
                RuleFor(x => x.AccountUserName)
                    .Matches(@".{8,}").WithMessage("{PropertyName}MustHaveLeast8Characters")
                    .MaximumLength(255).WithMessage("{PropertyName}MustHaveLessThan255Characters")
                    .MustAsync(async (x, y, z) => { return await AccountService.IsUserNameDoesNotExistsValidationAsync(x.AccountUserName, accountId); }).WithMessage("{PropertyName}AlreadyExists")
                    .WithName("AccountUserName");
            });
            When(x => !string.IsNullOrEmpty(x.AccountPassword), () =>
            {
                RuleFor(x => x.AccountPassword)
               .Matches(@"[^a-zA-Z0-9]+").WithMessage("{PropertyName}MustIncludeSymbol")
               .Matches(@".{8,}").WithMessage("{PropertyName}MustHaveLeast8Characters")
               .Matches(@"[0-9]+").WithMessage("{PropertyName}MustIncludeNumericCharacters")
               .Matches(@"[A-Z]+").WithMessage("{PropertyName}MustIncludeUppercase")
               .Matches(@"[a-z]+").WithMessage("{PropertyName}MustIncludeLowercase")
               .WithName("AccountPassword");
            });
            When(x => x.AccountAvatar is not null, () =>
            {
                RuleFor(x => x.AccountAvatar)
                .Must((x, y) => { return AccountService.IsValidContentType(x.AccountAvatar, new List<string> { "image" }); }).WithMessage("{PropertyName}ContentTypeIsNotSupportedUpload")
                .WithName("AccountAvatar");
            });
            When(x => x.PermissionCodes is not null && x.PermissionCodes.Count != 0 && x.PermissionCodes.Any(y => y is not null), () =>
            {
                RuleFor(x => x.PermissionCodes)
                .MustAsync(async (x, y, z) => { return await accountService.IsPermissionCodesExistsValidationAsync(x.PermissionCodes); }).WithMessage("{PropertyName}Invalid")
                .WithName("PermissionCodes");
            });
            When(x => x.AccountStatus is not null, () =>
            {
                RuleFor(x => x.AccountStatus)
                .IsInEnum().WithMessage("{PropertyName}InValid")
                .WithName("AccountStatus");
            });
            When(x => x.AccountTwoFactorAuth is not null, () =>
            {
                RuleFor(x => x.AccountTwoFactorAuth)
                .IsInEnum().WithMessage("{PropertyName}InValid")
                .WithName("AccountTwoFactorAuth");
            });
        }
    }
}
