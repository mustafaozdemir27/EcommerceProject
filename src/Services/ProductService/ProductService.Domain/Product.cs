// ProductService.Domain/Product.cs
using Common.Domain;
using ProductService.Domain.Events; // Bu using'i ekleyin
using ProductService.Domain.ValueObjects;

namespace ProductService.Domain
{
    public class Product : Entity<Guid>, IAggregateRoot
    {
        // ... (Özellikler ve private constructor) ...
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public Price CurrentPrice { get; private set; }
        public int StockQuantity { get; private set; }
        public Guid CategoryId { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime? UpdatedAtUtc { get; private set; }

        private Product() : base()
        {
            Name = null!;
            CurrentPrice = null!;
        }

        public Product(Guid id, string name, string? description, Price price, int initialStock, Guid categoryId) : base(id)
        {
            // ... (Mevcut doğrulamalar) ...
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name), "Ürün adı boş olamaz.");
            if (price == null)
                throw new ArgumentNullException(nameof(price), "Fiyat bilgisi boş olamaz.");
            if (initialStock < 0)
                throw new ArgumentOutOfRangeException(nameof(initialStock), "Stok miktarı negatif olamaz.");

            Name = name;
            Description = description;
            CurrentPrice = price;
            StockQuantity = initialStock;
            CategoryId = categoryId;
            CreatedAtUtc = DateTime.UtcNow;

            // ProductCreatedDomainEvent yayınla
            AddDomainEvent(new ProductCreatedDomainEvent(this.Id, this.Name, this.CurrentPrice.Amount, this.CurrentPrice.Currency, this.StockQuantity));
        }

        // ... (UpdateDetails metodu) ...
        public void UpdateDetails(string newName, string? newDescription, Guid newCategoryId)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentNullException(nameof(newName), "Yeni ürün adı boş olamaz.");

            bool changed = false;
            if (Name != newName) { Name = newName; changed = true; }
            if (Description != newDescription) { Description = newDescription; changed = true; }
            if (CategoryId != newCategoryId) { CategoryId = newCategoryId; changed = true; }

            if (changed)
            {
                UpdatedAtUtc = DateTime.UtcNow;
                // TODO: ProductDetailsUpdatedDomainEvent yayınla (şimdilik pas geçiyoruz)
            }
        }


        public void ChangePrice(Price newPrice)
        {
            if (newPrice == null)
                throw new ArgumentNullException(nameof(newPrice), "Yeni fiyat bilgisi boş olamaz.");
            if (CurrentPrice.Equals(newPrice))
                return;

            var oldPrice = CurrentPrice; // Eski fiyatı sakla
            CurrentPrice = newPrice;
            UpdatedAtUtc = DateTime.UtcNow;

            // ProductPriceChangedDomainEvent yayınla
            AddDomainEvent(new ProductPriceChangedDomainEvent(this.Id, oldPrice, this.CurrentPrice));
        }

        // ... (AddStock ve RemoveStock metotları) ...
        // Bu metotlar için de benzer şekilde domain olayları eklenebilir (StockIncreased/DecreasedDomainEvent)
        // Şimdilik basit tutmak adına onları pas geçiyoruz.
        public void AddStock(int quantityToAdd)
        {
            if (quantityToAdd <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantityToAdd), "Eklenecek stok miktarı pozitif olmalıdır.");

            StockQuantity += quantityToAdd;
            UpdatedAtUtc = DateTime.UtcNow;
            // TODO: StockIncreasedDomainEvent yayınla
        }

        public void RemoveStock(int quantityToRemove)
        {
            if (quantityToRemove <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantityToRemove), "Azaltılacak stok miktarı pozitif olmalıdır.");
            if (StockQuantity < quantityToRemove)
                throw new InvalidOperationException("Yetersiz stok.");

            StockQuantity -= quantityToRemove;
            UpdatedAtUtc = DateTime.UtcNow;
            // TODO: StockDecreasedDomainEvent yayınla
        }
    }
}