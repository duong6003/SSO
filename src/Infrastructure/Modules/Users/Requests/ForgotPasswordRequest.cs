using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Persistence.Repositories;
namespace Infrastructure.Modules.Users.Requests
{
    public class ForgotPasswordRequest
    {
        public string? Email { get; set; }
    }
    public class ForgotPasswordValidation : AbstractValidator<ForgotPasswordRequest>
    {

        public ForgotPasswordValidation(IRepositoryWrapper repositoryWrapper)
        {
            RuleFor(x => x.Email)
                .MustAsync(async (email, cancellationToken) => await repositoryWrapper.Users.AnyAsync(x => x.EmailAddress == email)).WithMessage(Messages.Users.UserNameNotExist);
        }
    }
}