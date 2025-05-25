// OrderService.Domain/Events/OrderCreatedDomainEvent.cs
using Common.Domain; // IDomainEvent için
using OrderService.Domain.Enums; // OrderStatus için

namespace OrderService.Domain.Events
{
    public class OrderCreatedDomainEvent : IDomainEvent
    {
        public Guid OrderId { get; }
        public Guid CustomerId { get; }
        public DateTime OrderDate { get; }
        public decimal TotalPrice { get; } // Toplam fiyatı da olaya ekleyelim
        public OrderStatus InitialStatus { get; }
        // public IEnumerable<OrderItemSnapshot> Items { get; } // Sipariş kalemlerinin anlık görüntüsü (opsiyonel)

        public DateTime OccurredOn { get; }

        public OrderCreatedDomainEvent(Guid orderId, Guid customerId, DateTime orderDate, decimal totalPrice, OrderStatus initialStatus)
        {
            OrderId = orderId;
            CustomerId = customerId;
            OrderDate = orderDate;
            TotalPrice = totalPrice;
            InitialStatus = initialStatus;
            OccurredOn = DateTime.UtcNow;
        }
    }

    // Opsiyonel: Sipariş kalemlerinin temel bilgilerini taşımak için bir snapshot DTO'su
    // public class OrderItemSnapshot
    // {
    //     public Guid ProductId { get; set; }
    //     public int Quantity { get; set; }
    //     public decimal UnitPrice { get; set; }
    // }
}