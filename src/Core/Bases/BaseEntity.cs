namespace Core.Bases;

public abstract class BaseEntity : BaseEntity<Guid>
{
    protected BaseEntity() => (Id, CreatedAt) = (Guid.NewGuid(), DateTimeOffset.UtcNow);
}

public abstract class BaseEntity<TId>
{
    public TId Id { get; protected set; } = default!;
    public DateTimeOffset CreatedAt { get; protected set; }
}
