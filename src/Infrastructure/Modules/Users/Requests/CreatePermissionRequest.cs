using FluentValidation;
using Infrastructure.Definitions;
using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Modules.Users.Requests
{
    public class CreatePermissionRequest
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
    }
    public class CreatePermissionValidation : AbstractValidator<CreatePermissionRequest>
    {

        public CreatePermissionValidation(IRepositoryWrapper repositoryWrapper)
        {
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage(Messages.Permissions.CodeNotEmpty)
                .MustAsync(async (code, cancellationToken) => await repositoryWrapper.Permissions.GetByIdAsync(code) != null)
                .WithMessage(Messages.Permissions.CodeAlreadyExist);
            RuleFor(x => x.Name).NotEmpty().WithMessage(Messages.Permissions.NameNotEmpty);
        }
    }
}