// ProductService.Domain/Repositories/IProductRepository.cs
namespace ProductService.Domain.Repositories
{
    public interface IProductRepository
    {
        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default);
        // İleride kategoriye göre ürünleri getirme gibi metotlar eklenebilir:
        // Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
        Task AddAsync(Product product, CancellationToken cancellationToken = default);
        void Update(Product product); // EF Core değişiklik izleyicisi nedeniyle genellikle senkron
        Task DeleteAsync(Product product, CancellationToken cancellationToken = default); // Veya sadece ID ile silme
    }
}