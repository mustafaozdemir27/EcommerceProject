// ProductService.Application/Features/Products/Dtos/ProductDto.cs
namespace ProductService.Application.Features.Products.Dtos
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal PriceAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public int StockQuantity { get; set; }
        public Guid CategoryId { get; set; }
        // İleride Kategori Adı gibi ilişkili veriler de buraya eklenebilir
        // public string CategoryName { get; set; } = string.Empty; 
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
    }
}