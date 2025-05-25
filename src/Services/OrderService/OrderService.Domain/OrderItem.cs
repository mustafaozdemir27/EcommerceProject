// OrderService.Domain/OrderItem.cs
using Common.Domain; // Entity<TId> için
// ProductService.Domain.ValueObjects.Price; // Eğer Price VO'sunu doğrudan kullanacaksak,
// ama genellikle sipariş anındaki fiyatı kopyalarız.

namespace OrderService.Domain
{
    public class OrderItem : Entity<Guid> // Her sipariş kaleminin kendi ID'si olabilir
    {
        public Guid ProductId { get; private set; }
        public string ProductName { get; private set; } // Sipariş anındaki ürün adı (değişebilir diye kopyalanır)
        public decimal UnitPrice { get; private set; }   // Sipariş anındaki birim fiyat
        public string Currency { get; private set; }    // Sipariş anındaki para birimi
        public int Quantity { get; private set; }

        public Guid OrderId { get; private set; } // Hangi siparişe ait olduğu

        // EF Core için
        private OrderItem() : base()
        {
            ProductName = null!;
            Currency = null!;
        }

        public OrderItem(Guid productId, string productName, decimal unitPrice, string currency, int quantity)
            : base(Guid.NewGuid()) // Yeni bir ID ata
        {
            if (productId == Guid.Empty)
                throw new ArgumentException("Ürün ID'si boş olamaz.", nameof(productId));
            if (string.IsNullOrWhiteSpace(productName))
                throw new ArgumentNullException(nameof(productName), "Ürün adı boş olamaz.");
            if (unitPrice <= 0)
                throw new ArgumentOutOfRangeException(nameof(unitPrice), "Birim fiyat pozitif olmalıdır.");
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentNullException(nameof(currency), "Para birimi boş olamaz.");
            if (quantity <= 0)
                throw new ArgumentOutOfRangeException(nameof(quantity), "Miktar pozitif olmalıdır.");

            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Currency = currency;
            Quantity = quantity;
        }

        // Sipariş kalemiyle ilgili metotlar eklenebilir, örneğin miktarı artırma/azaltma (stok kontrolüyle entegre)
        // public void UpdateQuantity(int newQuantity) { ... }

        public decimal GetTotalPrice() => UnitPrice * Quantity;
    }
}