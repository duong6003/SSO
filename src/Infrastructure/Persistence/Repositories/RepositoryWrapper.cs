using Core.Common.Interfaces;
using Infrastructure.Modules.Users.Entities;
using Infrastructure.Persistence.Contexts;

namespace Infrastructure.Persistence.Repositories;

public interface IRepositoryWrapper : IScopedService
{
    IRepositoryBase<User> Users { get; }
    IRepositoryBase<RolePermission> RolePermissions { get; }
    IRepositoryBase<Permission> Permissions { get; }
    IRepositoryBase<UserPermission> UserPermissions { get; }
    IRepositoryBase<Role> Roles { get; }

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

public class RepositoryWrapper : IRepositoryWrapper
{
    private readonly ApplicationDbContext ApplicationDbContext;
    public RepositoryWrapper(ApplicationDbContext applicationDbContext) => ApplicationDbContext = applicationDbContext;


    private IRepositoryBase<User>? UsersRepositoryBase;
    public IRepositoryBase<User> Users => UsersRepositoryBase ??= new RepositoryBase<User>(ApplicationDbContext);

    private IRepositoryBase<RolePermission>? RolePermissionsRepositoryBase;
    public IRepositoryBase<RolePermission> RolePermissions => RolePermissionsRepositoryBase ??= new RepositoryBase<RolePermission>(ApplicationDbContext);

    private IRepositoryBase<Permission>? PermissionsRepositoryBase;
    public IRepositoryBase<Permission> Permissions => PermissionsRepositoryBase ??= new RepositoryBase<Permission>(ApplicationDbContext);

    private IRepositoryBase<UserPermission>? UserPermissionsRepositoryBase;
    public IRepositoryBase<UserPermission> UserPermissions => UserPermissionsRepositoryBase ??= new RepositoryBase<UserPermission>(ApplicationDbContext);

    private IRepositoryBase<Role>? RolesRepositoryBase;
    public IRepositoryBase<Role> Roles => RolesRepositoryBase ??= new RepositoryBase<Role>(ApplicationDbContext);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default) => await ApplicationDbContext.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default) => await ApplicationDbContext.Database.CommitTransactionAsync(cancellationToken);

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default) => await ApplicationDbContext.Database.RollbackTransactionAsync(cancellationToken);
}
