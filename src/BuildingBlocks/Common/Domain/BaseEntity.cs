namespace Hello100Admin.BuildingBlocks.Common.Domain;

/// <summary>
/// 모든 엔티티의 기본 클래스 (제네릭 ID 타입)
/// </summary>
public abstract class BaseEntity<TId> where TId : notnull
{
    public TId Id { get; protected set; } = default!;
    public string? CreatedBy { get; protected set; }
    public string? UpdatedBy { get; protected set; }
    public bool IsDeleted { get; protected set; }

    private List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected BaseEntity()
    {
        IsDeleted = false;
    }

    public void AddDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Add(eventItem);
    }

    public void RemoveDomainEvent(IDomainEvent eventItem)
    {
        _domainEvents.Remove(eventItem);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    public void Delete()
    {
        IsDeleted = true;
    }
}

/// <summary>
/// Guid를 사용하는 엔티티의 기본 클래스 (기존 호환성 유지)
/// </summary>
public abstract class BaseEntity : BaseEntity<Guid>
{
    protected BaseEntity()
    {
        Id = Guid.NewGuid();
    }
}
