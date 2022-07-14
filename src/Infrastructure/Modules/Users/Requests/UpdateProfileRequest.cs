using FluentValidation;
using Infrastructure.Common.Validations;
using Infrastructure.Definitions;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Security.Claims;

namespace Infrastructure.Modules.Users.Requests
{
    public class UpdateProfileRequest
    {
        public string? FullName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public IFormFile? Avatar { get; set; }
    }
    public class UpdateProfileValidation : AbstractValidator<UpdateProfileRequest>
    {
        public UpdateProfileValidation(IRepositoryWrapper repositoryWrapper, IActionContextAccessor actionContextAccessor)
        {
            Guid.TryParse(actionContextAccessor.ActionContext!.HttpContext.User.FindFirstValue(JwtClaimsName.Identification), out Guid userId);
            RuleFor(x => x.EmailAddress)
                .MustAsync(async (userReq, emailAddress, cancellationToken) =>
                !await repositoryWrapper.Users.AnyAsync(x => x.EmailAddress == emailAddress && x.Id != userId))
                .WithMessage(Messages.Users.EmailAddressAlreadyExist)
                .EmailAddress().WithMessage(Messages.Users.EmailAddressInvalid);
            When(x => !string.IsNullOrEmpty(x.FullName) , () =>
            {
                RuleFor(x => x.FullName).MaximumLength(128).WithMessage(Messages.Users.FullNameOverLength);
            });
            When(x => !string.IsNullOrWhiteSpace(x.Password) && !string.IsNullOrWhiteSpace(x.ConfirmPassword), () => 
            {
                RuleFor(x => x.Password)
                .IsValidPassword().When(x => x.Password != null).WithMessage(Messages.Users.PasswordInValid);
                RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage(Messages.Users.PasswordConfirmNotMatch);
            });
            RuleFor(x => x.Avatar)
                .IsValidContentType(new List<string>() { "image" }).WithMessage(Messages.Users.AvatarInValid);
        }
    }
}