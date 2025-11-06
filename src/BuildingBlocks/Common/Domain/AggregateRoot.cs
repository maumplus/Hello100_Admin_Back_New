namespace Hello100Admin.BuildingBlocks.Common.Domain;

/// <summary>
/// 애그리게이트 루트 기본 클래스 (제네릭 버전)
/// </summary>
public abstract class AggregateRoot<TId> : BaseEntity<TId> where TId : notnull
{
    public int Version { get; protected set; }

    protected AggregateRoot()
    {
        Version = 0;
    }

    public void IncrementVersion()
    {
        Version++;
    }
}

/// <summary>
/// 애그리게이트 루트 기본 클래스 (Guid 버전 - 기존 호환성)
/// </summary>
public abstract class AggregateRoot : AggregateRoot<Guid>
{
}
