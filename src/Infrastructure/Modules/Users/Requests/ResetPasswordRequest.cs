using FluentValidation;
using Infrastructure.Common.Validations;
using Infrastructure.Definitions;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Infrastructure.Modules.Users.Requests
{
    public class ResetPasswordRequest
    {
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
    }
    public class ResetPasswordValidation : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordValidation(IRepositoryWrapper repositoryWrapper, IActionContextAccessor actionContextAccessor)
        {
            Guid.TryParse(actionContextAccessor.ActionContext!.RouteData.Values
                .FirstOrDefault(x => x.Key.Equals("userId"))
                .Value?
                .ToString(), out Guid userId);
            RuleFor(x => x)
                .MustAsync(async (request, cancellationToken) => await repositoryWrapper.Permissions.GetByIdAsync(userId) != null)
                .WithMessage(Messages.Users.IdNotFound);
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage(Messages.Users.PasswordNotEmpty)
                .MaximumLength(128).WithMessage(Messages.Users.PasswordOverLength)
                .IsValidPassword().WithMessage(Messages.Users.PasswordInValid);
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage(Messages.Users.PasswordConfirmNotMatch);
        }
    }
}