using MediatR;

namespace Hello100Admin.BuildingBlocks.Common.Domain;

/// <summary>
/// 도메인 이벤트 인터페이스
/// MediatR의 INotification을 상속하여 이벤트 발행 지원
/// </summary>
public interface IDomainEvent : INotification
{
    Guid EventId => Guid.NewGuid();
    DateTime OccurredOn => DateTime.UtcNow;
}
