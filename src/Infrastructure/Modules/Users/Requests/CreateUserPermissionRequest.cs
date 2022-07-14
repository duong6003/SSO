using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Infrastructure.Modules.Users.Requests
{
    public class CreateUserPermissionRequest
    {
        public string? Code { get; set; }
    }
    public class CreateUserPermissionValidation : AbstractValidator<List<CreateUserPermissionRequest>>
    {
        public CreateUserPermissionValidation(IRepositoryWrapper repositoryWrapper, IActionContextAccessor actionContextAccessor)
        {
           Guid.TryParse(actionContextAccessor.ActionContext!.RouteData.Values
                .FirstOrDefault(x => x.Key.Equals("userId"))
                .Value?
                .ToString(), out Guid userId);
            RuleForEach(x => x)
                .MustAsync(async (request, cancellationToken) => 
                    await repositoryWrapper.Users.GetByIdAsync(userId) != null
                )
                .WithMessage(Messages.Users.IdNotFound)
                .MustAsync(async (request, cancellationToken) =>
                    await repositoryWrapper.Permissions.GetByIdAsync(request.Code) != null
                )
                .WithMessage(Messages.Permissions.CodeNotFound)
                .MustAsync(async (request, cancellationToken) => await repositoryWrapper.UserPermissions.GetByIdAsync(userId, request.Code) == null)
                .WithMessage(Messages.UserPermissions.AlreadyExist);
        }
    }
}