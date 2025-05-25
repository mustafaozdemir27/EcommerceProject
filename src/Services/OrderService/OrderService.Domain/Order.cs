// OrderService.Domain/Order.cs
using Common.Domain;
using OrderService.Domain.Enums;
using OrderService.Domain.Events; // Bu using'i ekleyin

namespace OrderService.Domain
{
    public class Order : Entity<Guid>, IAggregateRoot
    {
        // ... (Özellikler) ...
        public Guid CustomerId { get; private set; }
        private readonly List<OrderItem> _orderItems;
        public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();
        public DateTime OrderDateUtc { get; private set; }
        public OrderStatus Status { get; private set; }
        public string ShippingAddressStreet { get; private set; }
        public string ShippingAddressCity { get; private set; }
        public string ShippingAddressCountry { get; private set; }
        public string ShippingAddressZipCode { get; private set; }

        private Order() : base()
        {
            _orderItems = new List<OrderItem>();
            ShippingAddressStreet = null!;
            ShippingAddressCity = null!;
            ShippingAddressCountry = null!;
            ShippingAddressZipCode = null!;
        }

        public Order(Guid customerId, string shippingStreet, string shippingCity, string shippingCountry, string shippingZipCode)
            : base(Guid.NewGuid())
        {
            // ... (Mevcut doğrulamalar) ...
            CustomerId = customerId;
            _orderItems = new List<OrderItem>();
            OrderDateUtc = DateTime.UtcNow;
            Status = OrderStatus.Pending;

            ShippingAddressStreet = shippingStreet;
            ShippingAddressCity = shippingCity;
            ShippingAddressCountry = shippingCountry;
            ShippingAddressZipCode = shippingZipCode;

            // OrderCreatedDomainEvent yayınla
            AddDomainEvent(new OrderCreatedDomainEvent(this.Id, this.CustomerId, this.OrderDateUtc, 0, this.Status));
            // TotalPrice başlangıçta 0, AddOrderItem ile güncellenecek veya GetTotalPrice() ile hesaplanacak.
            // Olayda başlangıç fiyatını veya kalemleri taşımak da bir seçenek.
        }

        public void AddOrderItem(Guid productId, string productName, decimal unitPrice, string currency, int quantity)
        {
            if (Status != OrderStatus.Pending && Status != OrderStatus.Processing) // Belki Processing durumunda da eklenebilir
                throw new InvalidOperationException("Sadece 'Beklemede' veya 'İşleniyor' durumundaki siparişlere ürün eklenebilir.");

            // ... (Mevcut AddOrderItem mantığı) ...
            var existingItem = _orderItems.FirstOrDefault(item => item.ProductId == productId);
            if (existingItem != null)
            {
                throw new InvalidOperationException("Bu ürün zaten siparişte mevcut. Miktarını güncelleyin.");
            }
            else
            {
                var newItem = new OrderItem(productId, productName, unitPrice, currency, quantity);
                _orderItems.Add(newItem);
            }
            // Olay: OrderItemAddedToOrderDomainEvent gibi bir olay da eklenebilir.
            // Veya OrderUpdated gibi genel bir olay. Şimdilik OrderCreated'a odaklanalım.
        }

        public decimal GetTotalPrice()
        {
            return _orderItems.Sum(item => item.GetTotalPrice());
        }

        private void SetStatus(OrderStatus newStatus)
        {
            if (Status == newStatus) return;

            var oldStatus = Status;
            Status = newStatus;
            AddDomainEvent(new OrderStatusChangedDomainEvent(this.Id, oldStatus, newStatus));
        }

        public void SetStatusToProcessing()
        {
            if (Status == OrderStatus.Pending)
            {
                SetStatus(OrderStatus.Processing);
            }
            else
            {
                throw new InvalidOperationException($"Sipariş durumu '{Status}' iken '{OrderStatus.Processing}' durumuna geçirilemez.");
            }
        }

        public void SetStatusToShipped()
        {
            if (Status == OrderStatus.Processing)
            {
                SetStatus(OrderStatus.Shipped);
            }
            else
            {
                throw new InvalidOperationException($"Sipariş durumu '{Status}' iken '{OrderStatus.Shipped}' durumuna geçirilemez.");
            }
        }

        public void SetStatusToDelivered()
        {
            if (Status == OrderStatus.Shipped)
            {
                SetStatus(OrderStatus.Delivered);
            }
            else
            {
                throw new InvalidOperationException($"Sipariş durumu '{Status}' iken '{OrderStatus.Delivered}' durumuna geçirilemez.");
            }
        }

        public void CancelOrder() // İptal etme
        {
            // İptal etme kuralları (örneğin sadece belirli durumlardayken iptal edilebilir)
            if (Status == OrderStatus.Delivered || Status == OrderStatus.Shipped)
            {
                throw new InvalidOperationException($"Sipariş durumu '{Status}' iken iptal edilemez.");
            }
            if (Status != OrderStatus.Cancelled) // Zaten iptalse tekrar işlem yapma
            {
                SetStatus(OrderStatus.Cancelled);
            }
        }
    }
}