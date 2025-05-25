// OrderService.Application/Features/Orders/Dtos/OrderDto.cs
// using OrderService.Domain.Enums; // OrderStatus için, doğrudan string de kullanılabilir

namespace OrderService.Application.Features.Orders.Dtos
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime OrderDateUtc { get; set; }
        public string Status { get; set; } = string.Empty; // OrderStatus enum'ının string karşılığı
        public decimal TotalPrice { get; set; }

        // Adres Bilgileri
        public string ShippingAddressStreet { get; set; } = string.Empty;
        public string ShippingAddressCity { get; set; } = string.Empty;
        public string ShippingAddressCountry { get; set; } = string.Empty;
        public string ShippingAddressZipCode { get; set; } = string.Empty;

        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
    }
}