using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Infrastructure.Modules.Users.Requests
{
    public class UpdateRoleRequest
    {
        public string? Name { get; set; }
        public List<UpdateRolePermissionRequest>? RolePermissions { get; set; }
    }
    public class UpdateRoleValidation : AbstractValidator<UpdateRoleRequest>
    {
        public UpdateRoleValidation(IRepositoryWrapper repositoryWrapper, IActionContextAccessor actionContextAccessor)
        {
            Guid.TryParse(actionContextAccessor.ActionContext!.RouteData.Values
                .FirstOrDefault(x => x.Key.Equals("roleId"))
                .Value?
                .ToString(), out Guid roleId);
            RuleFor(x => x)
                .MustAsync(async (request, cancellationToken) => await repositoryWrapper.Roles.GetByIdAsync(roleId) != null)
                .WithMessage(Messages.Roles.IdNotFound);
            RuleForEach(x => x.RolePermissions).Cascade(CascadeMode.Stop).SetValidator(new UpdateRolePermissionValidation(repositoryWrapper));
        }
    }
}