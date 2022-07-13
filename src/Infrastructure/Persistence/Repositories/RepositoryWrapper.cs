using Core.Common.Interfaces;
using Infrastructure.Modules.Accounts.Entities;
using Infrastructure.Modules.Applications.Entities;
using Infrastructure.Persistence.Contexts;

namespace Infrastructure.Persistence.Repositories;

public partial interface IRepositoryWrapper : IScopedService
{
    IRepositoryBase<Account> Accounts { get; }
    IRepositoryBase<Application> Applications { get; }

    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

public partial class RepositoryWrapper : IRepositoryWrapper
{
    private readonly ApplicationDbContext ApplicationDbContext;Application
    public RepositoryWrapper(ApplicationDbContext applicationDbContext) => ApplicationDbContext = applicationDbContext;

    private IRepositoryBase<Account>? AccountsRepositoryBase;
    public IRepositoryBase<Account> Accounts => AccountsRepositoryBase ??= new RepositoryBase<Account>(ApplicationDbContext);

    private IRepositoryBase<Application>? ApplicationsRepositoryBase;
    public IRepositoryBase<Application> Applications => ApplicationsRepositoryBase ??= new RepositoryBase<Application>(ApplicationDbContext);

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default) => await ApplicationDbContext.Database.BeginTransactionAsync(cancellationToken);

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default) => await ApplicationDbContext.Database.CommitTransactionAsync(cancellationToken);

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default) => await ApplicationDbContext.Database.RollbackTransactionAsync(cancellationToken);
}
