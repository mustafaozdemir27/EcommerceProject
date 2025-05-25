// OrderService.Application/Features/Orders/Commands/CreateOrder/CreateOrderCommandHandler.cs
using Common.Infrastructure.Data;
using MediatR;
using OrderService.Domain;                   // Order, OrderItem entity'leri için
using OrderService.Domain.Repositories;       // IOrderRepository için
// using Microsoft.Extensions.Logging;      // ILogger (opsiyonel, loglama için)
// using ProductService.Contracts; // Eğer ProductService ile iletişim için bir client/contract projesi varsa (ileride)

namespace OrderService.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUnitOfWork _unitOfWork;
        // private readonly IProductServiceIntegration _productService; // İleride eklenecek
        // private readonly ILogger<CreateOrderCommandHandler> _logger; // Opsiyonel

        public CreateOrderCommandHandler(
            IOrderRepository orderRepository,
            IUnitOfWork unitOfWork
            /*, IProductServiceIntegration productService, ILogger<CreateOrderCommandHandler> logger */)
        {
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            // _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            // _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            // Validasyonun FluentValidation tarafından zaten yapıldığını varsayıyoruz.

            // Yeni Order entity'sini oluştur
            var newOrder = new Order(
                request.CustomerId,
                request.ShippingAddressStreet,
                request.ShippingAddressCity,
                request.ShippingAddressCountry,
                request.ShippingAddressZipCode
            );
            // Order constructor'ı içinde OrderCreatedDomainEvent zaten AddDomainEvent ile ekleniyor.
            // (TotalPrice henüz 0, kalemler eklendikçe güncellenecek veya GetTotalPrice() ile hesaplanacak)

            // Sipariş kalemlerini (OrderItems) Order entity'sine ekle
            foreach (var itemDto in request.OrderItems)
            {
                // TODO (İLERİKİ ADIM - Servisler Arası İletişim):
                // 1. itemDto.ProductId kullanarak ProductService'ten ürünün adını, güncel fiyatını ve para birimini al.
                // 2. Stok kontrolü yap.
                // Şimdilik bu bilgileri ya DTO'dan (eğer gönderildiyse) ya da sabit/varsayılan değerlerle alıyoruz.
                // Burada ProductName ve UnitPrice'ı DTO'dan almak yerine ProductService'ten almak daha doğru olur.
                // Basitlik adına şimdilik DTO'da bu alanlar olmadığını varsayarak sabit değerler kullanalım
                // veya ProductId'yi ProductName olarak kullanalım (sadece test için).

                var productName = $"Ürün-{itemDto.ProductId.ToString().Substring(0, 8)}"; // GEÇİCİ
                var unitPrice = 10.0m; // GEÇİCİ - ProductService'ten alınmalı
                var currency = "TRY";  // GEÇİCİ - ProductService'ten alınmalı

                if (itemDto.Quantity <= 0) // Validator'da da vardı ama burada da kontrol etmek iyi olabilir.
                {
                    // Belki burada bir exception fırlatmak veya loglamak gerekebilir.
                    // Şimdilik devam edelim, validator'a güveniyoruz.
                }

                newOrder.AddOrderItem(
                    itemDto.ProductId,
                    productName, // ProductService'ten alınacak gerçek ürün adı
                    unitPrice,   // ProductService'ten alınacak gerçek birim fiyat
                    currency,    // ProductService'ten alınacak gerçek para birimi
                    itemDto.Quantity
                );
            }

            // Siparişi repository'ye ekle
            await _orderRepository.AddAsync(newOrder, cancellationToken);

            // Değişiklikleri kaydet (Unit of Work)
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // _logger?.LogInformation("Yeni sipariş oluşturuldu: {OrderId}", newOrder.Id);

            // Yeni siparişin ID'sini döndür
            return newOrder.Id;
        }
    }
}