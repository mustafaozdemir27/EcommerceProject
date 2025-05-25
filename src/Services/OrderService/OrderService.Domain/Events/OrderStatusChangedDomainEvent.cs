// OrderService.Domain/Events/OrderStatusChangedDomainEvent.cs
using Common.Domain;
using OrderService.Domain.Enums;

namespace OrderService.Domain.Events
{
    public class OrderStatusChangedDomainEvent : IDomainEvent
    {
        public Guid OrderId { get; }
        public OrderStatus PreviousStatus { get; }
        public OrderStatus NewStatus { get; }
        public DateTime OccurredOn { get; }

        public OrderStatusChangedDomainEvent(Guid orderId, OrderStatus previousStatus, OrderStatus newStatus)
        {
            OrderId = orderId;
            PreviousStatus = previousStatus;
            NewStatus = newStatus;
            OccurredOn = DateTime.UtcNow;
        }
    }
}