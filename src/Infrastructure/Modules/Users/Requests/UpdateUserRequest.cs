using FluentValidation;
using Infrastructure.Common.Validations;
using Infrastructure.Definitions;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Infrastructure.Modules.Users.Requests
{
    public class UpdateUserRequest
    {
        public string? FullName { get; set; }
        public string? EmailAddress { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public bool? Active { get; set; }
        public IFormFile? Avatar { get; set; }
        public Guid? RoleId { get; set; }
    }
    public class UpdateUserValidation : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserValidation(IRepositoryWrapper repositoryWrapper, IActionContextAccessor actionContextAccessor)
        {
            Guid.TryParse(actionContextAccessor.ActionContext!.RouteData.Values
                .FirstOrDefault(x => x.Key.Equals("userId"))
                .Value?
                .ToString(), out Guid userId);
            RuleFor(x => x)
           .MustAsync(async (request, cancellationToken) => await repositoryWrapper.Users.GetByIdAsync(userId) != null)
           .WithMessage(Messages.Users.IdNotFound);
            RuleFor(x => x.Avatar)
                .IsValidContentType(new List<string>() { "image" }).WithMessage(Messages.Users.AvatarInValid);
            When(x => !string.IsNullOrEmpty(x.FullName), () =>
            {
                RuleFor(x => x.FullName).MaximumLength(128).WithMessage(Messages.Users.FullNameOverLength);
            });
        }
    }
}