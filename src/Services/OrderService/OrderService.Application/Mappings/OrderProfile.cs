// OrderService.Application/Mappings/OrderProfile.cs
using AutoMapper;
using OrderService.Application.Features.Orders.Dtos; // OrderDto, OrderItemDto için
using OrderService.Domain;                         // Order, OrderItem entity'leri için

namespace OrderService.Application.Mappings
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            // OrderItem entity'sinden OrderItemDto'ya mapping
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.GetTotalPrice())); // Hesaplanan alanı maple

            // Order entity'sinden OrderDto'ya mapping
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString())) // Enum'ı string'e çevir
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.GetTotalPrice())); // Hesaplanan alanı maple
                                                                                                     // OrderItems koleksiyonu otomatik olarak OrderItem -> OrderItemDto mapping'i ile maplenir.

            // CreateOrderCommand içindeki OrderItemDto'dan OrderItem entity'sine mapping (Opsiyonel)
            // CreateOrderCommandHandler'da bu dönüşümü manuel yapıyoruz (ürün adı, fiyat gibi bilgileri
            // ProductService'ten alacağımızı varsayarak). Eğer DTO'dan doğrudan oluşturacaksak bu mapping gerekebilir.
            // CreateMap<Features.Orders.Commands.CreateOrder.OrderItemDto, OrderItem>()
            //    .ConstructUsing(dto => new OrderItem(
            //        dto.ProductId,
            //        "ProductName_TODO", // ProductService'ten alınacak
            //        0, // ProductService'ten alınacak UnitPrice
            //        "TRY", // ProductService'ten alınacak Currency
            //        dto.Quantity
            //    ));
        }
    }
}