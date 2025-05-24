// ProductService.Application/Features/Products/Commands/CreateProduct/CreateProductCommandHandler.cs
using Common.Infrastructure.Data;
using MediatR;
using ProductService.Domain;                   // Product entity için
using ProductService.Domain.Repositories;       // IProductRepository için
using ProductService.Domain.ValueObjects;       // Price Value Object için
// using Microsoft.Extensions.Logging;      // ILogger (opsiyonel, loglama için)

namespace ProductService.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
    {
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        // private readonly ILogger<CreateProductCommandHandler> _logger; // Opsiyonel

        public CreateProductCommandHandler(
            IProductRepository productRepository,
            IUnitOfWork unitOfWork
            /*, ILogger<CreateProductCommandHandler> logger */)
        {
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            // _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            // Validasyonun FluentValidation tarafından (MediatR pipeline behavior'ı veya
            // ASP.NET Core pipeline'ı aracılığıyla) zaten yapıldığını varsayıyoruz.
            // Eğer burada ek iş kuralları kontrolü gerekiyorsa (örn: kategori ID'sinin varlığı),
            // o kontroller burada yapılabilir veya bir Domain Service kullanılabilir.

            // Price Value Object'ini oluştur
            var price = new Price(request.PriceAmount, request.Currency);

            // Yeni Product entity'sini oluştur
            var newProduct = new Product(
                Guid.NewGuid(),         // Yeni ürün için ID oluştur
                request.Name,
                request.Description,
                price,
                request.InitialStock,
                request.CategoryId
            );
            // Product constructor'ı içinde ProductCreatedDomainEvent zaten AddDomainEvent ile ekleniyor.

            // Ürünü repository'ye ekle
            await _productRepository.AddAsync(newProduct, cancellationToken);

            // Değişiklikleri kaydet (Unit of Work)
            // Bu işlem, ürünü veritabanına kaydeder ve ProductCreatedDomainEvent'in
            // ProductDbContext üzerinden yayınlanmasını tetikler (eğer ProductDbContext'i
            // UserService'teki UserDbContext gibi olay yayınlayacak şekilde ayarlarsak).
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // _logger?.LogInformation("Yeni ürün oluşturuldu: {ProductId} - {ProductName}", newProduct.Id, newProduct.Name);

            // Yeni ürünün ID'sini döndür
            return newProduct.Id;
        }
    }
}