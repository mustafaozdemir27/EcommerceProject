// ProductService.Domain/Events/ProductPriceChangedDomainEvent.cs
using Common.Domain;
using ProductService.Domain.ValueObjects; // Price için

namespace ProductService.Domain.Events
{
    public class ProductPriceChangedDomainEvent : IDomainEvent
    {
        public Guid ProductId { get; }
        public Price OldPrice { get; }
        public Price NewPrice { get; }
        public DateTime OccurredOn { get; }

        public ProductPriceChangedDomainEvent(Guid productId, Price oldPrice, Price newPrice)
        {
            ProductId = productId;
            OldPrice = oldPrice;
            NewPrice = newPrice;
            OccurredOn = DateTime.UtcNow;
        }
    }
}