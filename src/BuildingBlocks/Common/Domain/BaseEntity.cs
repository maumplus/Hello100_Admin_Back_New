namespace Hello100Admin.BuildingBlocks.Common.Domain;

/// <summary>
/// 모든 엔티티의 기본 클래스 (제네릭 ID 타입)
/// </summary>
public abstract class BaseEntity<TId> where TId : notnull
{
    public TId Id { get; protected set; } = default!;
    
     /// <summary>
    /// 등록날짜 (Unix timestamp, int) -> TypeHandler에서 DateTime으로 변환
    /// </summary>
    public DateTime RegDt { get; set; }
    /// <summary>
    /// 수정날짜 (Unix timestamp, int) -> TypeHandler에서 DateTime으로 변환
    /// </summary>
    public DateTime ModDt { get; set; }
    public string? CreatedBy { get; protected set; }
    public string? UpdatedBy { get; protected set; }
    public bool IsDeleted { get; protected set; }

    private List<IDomainEvent> _domainEvents = new();
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected BaseEntity()
    {
        RegDt = DateTime.UtcNow;
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

    public void MarkAsDeleted()
    {
        IsDeleted = true;
        ModDt = DateTime.UtcNow;
    }

    public void UpdateModifiedInfo()
    {
        ModDt = DateTime.UtcNow;
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
