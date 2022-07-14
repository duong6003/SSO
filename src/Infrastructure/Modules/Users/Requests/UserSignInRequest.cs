using FluentValidation;
using Infrastructure.Common.Validations;
using Infrastructure.Definitions;
using Infrastructure.Modules.Users.Validations;
using Infrastructure.Persistence.Repositories;
namespace Infrastructure.Modules.Users.Requests
{
    public class UserSignInRequest
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
    }
    public class UserSignInValidation : AbstractValidator<UserSignInRequest>
    {
        public UserSignInValidation(IRepositoryWrapper repositoryWrapper)
        {
            IGlobalUserValidator globalUserValidator = new GlobalUserValidator(repositoryWrapper);
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage(Messages.Users.UserNameNotEmpty)
                .MaximumLength(255).WithMessage(Messages.Users.UserNameOverLength)
                .IsValidUserName().WithMessage(Messages.Users.UserNameInvalid)
                .MustAsync(async (userName, cancellationToken) => await repositoryWrapper.Users.AnyAsync(x => x.UserName == userName || x.EmailAddress == userName))
                .WithMessage(Messages.Users.UserNameNotExist)
                .MustAsync(async (userName, cancellationToken) => await globalUserValidator.IsActive(userName!))
                .WithMessage(Messages.Users.IsLocked);
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(Messages.Users.PasswordNotEmpty)
                .MaximumLength(255).WithMessage(Messages.Users.PasswordOverLength)
                .IsValidPassword().WithMessage(Messages.Users.PasswordInValid);
        }
    }
}