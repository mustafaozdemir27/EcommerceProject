// ProductService.Application/Features/Products/Commands/CreateProduct/CreateProductCommand.cs
using MediatR; // IRequest için
using System;   // Guid için

namespace ProductService.Application.Features.Products.Commands.CreateProduct
{
    public class CreateProductCommand : IRequest<Guid> // Guid: Yeni ürünün ID'sini döndürecek
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal PriceAmount { get; set; }
        public string Currency { get; set; } = "TRY"; // Varsayılan para birimi
        public int InitialStock { get; set; }
        public Guid CategoryId { get; set; }

        // Örnek: Eğer API'den fiyatı tek bir string olarak alıp burada parse etmek isterseniz:
        // public string PriceInput { get; set; }
        // Handler içinde PriceInput'u PriceAmount ve Currency'ye çevirebilirsiniz.
        // Ancak genellikle ayrı ayrı almak daha iyidir.
    }
}