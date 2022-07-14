using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Modules.Users.Requests
{
    public class UpdateRolePermissionRequest
    {
        public string? Code { get; set; }
    }
    public class UpdateRolePermissionValidation : AbstractValidator<UpdateRolePermissionRequest>
    {
        public UpdateRolePermissionValidation(IRepositoryWrapper repositoryWrapper)
        {
            RuleFor(x => x.Code)
                .NotNull().WithMessage(Messages.Permissions.CodeIsRequired)
                .MustAsync(async (code, cancellationToken) => await repositoryWrapper.Permissions.GetByIdAsync(code) != null)
                .WithMessage(Messages.Permissions.CodeNotFound);
        }
    }
}