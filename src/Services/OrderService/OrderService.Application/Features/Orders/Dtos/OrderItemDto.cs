namespace OrderService.Application.Features.Orders.Dtos
{
    public class OrderItemDto
    {
        public Guid Id { get; set; } // Sipariş kaleminin kendi ID'si (opsiyonel, entity'den maplerken gelir)
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty; // Sipariş anındaki ürün adı
        public decimal UnitPrice { get; set; }   // Sipariş anındaki birim fiyat
        public string Currency { get; set; } = string.Empty;    // Sipariş anındaki para birimi
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; } // UnitPrice * Quantity
    }
}
