using System.Diagnostics;
using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Modules.Users.Requests
{
    public class CreateRolePermissionRequest
    {
        public string? Code { get; set; }
    }
    public class CreateRolePermissionValidation : AbstractValidator<CreateRolePermissionRequest>
    {
        public CreateRolePermissionValidation(IRepositoryWrapper repositoryWrapper)
        {
            RuleFor(x => x.Code)
                .NotNull().WithMessage(Messages.Permissions.CodeIsRequired)
                .MustAsync(async (code, cancellationToken) => await repositoryWrapper.Permissions.GetByIdAsync(code) != null)
                .WithMessage(Messages.Permissions.CodeNotFound);
        }
    }
}