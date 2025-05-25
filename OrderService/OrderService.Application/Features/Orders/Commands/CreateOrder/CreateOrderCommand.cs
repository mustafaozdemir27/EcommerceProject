// OrderService.Application/Features/Orders/Commands/CreateOrder/CreateOrderCommand.cs
using MediatR;
using OrderService.Application.Features.Orders.Dtos; // IRequest için

namespace OrderService.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<Guid> // Guid: Yeni siparişin ID'sini döndürecek
    {
        public Guid CustomerId { get; set; }

        // Adres Bilgileri
        public string ShippingAddressStreet { get; set; } = string.Empty;
        public string ShippingAddressCity { get; set; } = string.Empty;
        public string ShippingAddressCountry { get; set; } = string.Empty;
        public string ShippingAddressZipCode { get; set; } = string.Empty;

        public List<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();

        // Sipariş Kalemleri için DTO
        // Bu DTO, API'den sipariş kalemlerini almak için kullanılacak.
        // ProductId, ProductName, UnitPrice, Currency gibi alanları içerebilir.
        // ProductName, UnitPrice ve Currency, sipariş anında ProductService'ten çekilecek
        // veya istemci tarafından (belki bir sepet objesinden) sağlanacak bilgilerdir.
        // Şimdilik sadece ProductId ve Quantity alalım, diğerlerini handler'da ProductService'ten çekeriz (ileriki adım).
    }
}