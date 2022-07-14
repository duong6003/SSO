using FluentValidation;
using Infrastructure.Common.Validations;
using Infrastructure.Definitions;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Modules.Users.Requests
{
    public class UserSignUpRequest
    {
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public IFormFile? Avatar { get; set; }
        public Guid? RoleId { get; set; }
    }
    public class UserSignUpValidation : AbstractValidator<UserSignUpRequest>
    {
        public UserSignUpValidation(IRepositoryWrapper repositoryWrapper)
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage(Messages.Users.UserNameNotEmpty)
                .MustAsync(async (userName, cancellationToken) => !await repositoryWrapper.Users.AnyAsync(x => x.UserName == userName)).WithMessage(Messages.Users.UserNameAlreadyExist)
                .IsValidUserName().WithMessage(Messages.Users.UserNameInvalid)
                .MaximumLength(50).WithMessage(Messages.Users.UserNameOverLength);
            RuleFor(x => x.FullName)
               .NotEmpty().WithMessage(Messages.Users.FullNameNotEmpty)
               .MaximumLength(128).WithMessage(Messages.Users.FullNameOverLength);
            RuleFor(x => x.EmailAddress)
                .MustAsync(async (emailAddress, cancellationToken) => !await repositoryWrapper.Users.AnyAsync(x => x.EmailAddress == emailAddress)).WithMessage(Messages.Users.EmailAddressAlreadyExist)
                .EmailAddress().WithMessage(Messages.Users.EmailAddressInvalid);
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(Messages.Users.PasswordNotEmpty)
                .MaximumLength(128).WithMessage(Messages.Users.PasswordOverLength)
                .IsValidPassword().WithMessage(Messages.Users.PasswordInValid);
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage(Messages.Users.PasswordConfirmNotMatch);
            RuleFor(x => x.Avatar)
                .IsValidContentType(new List<string>() { "image" }).WithMessage(Messages.Users.AvatarInValid);
            RuleFor(x => x.RoleId)
                .NotNull().WithMessage(Messages.Roles.IdIsRequired)
                .MustAsync(async (roleId, cancellationToken) => await repositoryWrapper.Roles.GetByIdAsync(roleId) != null).WithMessage(Messages.Roles.IdNotFound);
        }
    }
}