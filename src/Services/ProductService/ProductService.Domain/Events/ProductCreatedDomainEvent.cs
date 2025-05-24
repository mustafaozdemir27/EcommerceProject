// ProductService.Domain/Events/ProductCreatedDomainEvent.cs
using Common.Domain; // IDomainEvent için

namespace ProductService.Domain.Events
{
    public class ProductCreatedDomainEvent : IDomainEvent
    {
        public Guid ProductId { get; }
        public string Name { get; }
        public decimal PriceAmount { get; }
        public string PriceCurrency { get; }
        public int InitialStock { get; }
        public DateTime OccurredOn { get; }

        public ProductCreatedDomainEvent(Guid productId, string name, decimal priceAmount, string priceCurrency, int initialStock)
        {
            ProductId = productId;
            Name = name;
            PriceAmount = priceAmount;
            PriceCurrency = priceCurrency;
            InitialStock = initialStock;
            OccurredOn = DateTime.UtcNow;
        }
    }
}