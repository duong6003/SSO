using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Infrastructure.Modules.Users.Requests
{
    public class UpdatePermissionRequest
    {
        public string? Name { get; set; }
    }
    public class UpdatePermissionValidation : AbstractValidator<UpdatePermissionRequest>
    {
        public UpdatePermissionValidation(IRepositoryWrapper repositoryWrapper, IActionContextAccessor actionContextAccessor)
        {
            Guid.TryParse(actionContextAccessor.ActionContext!.RouteData.Values
                .FirstOrDefault(x => x.Key.Equals("code"))
                .Value?
                .ToString(), out Guid code);
            RuleFor(x => x)
           .MustAsync(async (request, cancellationToken) => await repositoryWrapper.Permissions.GetByIdAsync(code) != null)
           .WithMessage(Messages.Permissions.CodeNotFound);
            RuleFor(x => x.Name).NotEmpty().WithMessage(Messages.Permissions.NameNotEmpty);
        }
    }
}