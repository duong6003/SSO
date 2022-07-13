using FluentValidation;
using Infrastructure.Modules.Accounts.Requests;
using Infrastructure.Modules.Accounts.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Modules.Accounts.Validations
{
    public class AccountUpdateProfileValidation : AbstractValidator<AccountUpdateProfileRequest>
    {
        private readonly IAccountService AccountService;
        public AccountUpdateProfileValidation(IAccountService accountService)
        {
            AccountService = accountService;
            When(x => x.AccountAvatar is not null, () =>
            {
                RuleFor(x => x.AccountAvatar)
                .Must((x, y) => { return AccountService.IsValidContentType(x.AccountAvatar, new List<string> { "image" }); }).WithMessage("{PropertyName}ContentTypeIsNotSupportedUpload")
                .WithName("AccountAvatar");
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
