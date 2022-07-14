using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Infrastructure.Modules.Users.Requests
{
    public class UpdateUserPermissionRequest
    {
        public string? Code { get; set; }
    }
    public class UpdateUserPermissionValidation : AbstractValidator<List<UpdateUserPermissionRequest>>
    {
        public UpdateUserPermissionValidation(IRepositoryWrapper repositoryWrapper, IActionContextAccessor actionContextAccessor)
        {
            Guid.TryParse(actionContextAccessor.ActionContext!.RouteData.Values
                .FirstOrDefault(x => x.Key.Equals("userId"))
                .Value?
                .ToString(), out Guid userId);
            RuleForEach(x => x)
                .MustAsync(async (request, cancellationToken) => await repositoryWrapper.Permissions.GetByIdAsync(request.Code) != null)
                .WithMessage(Messages.Permissions.CodeNotFound)
                .MustAsync(async (request, cancellationToken) => await repositoryWrapper.Users.GetByIdAsync(userId) != null)
                .WithMessage(Messages.Users.IdNotFound);
        }
    }
}