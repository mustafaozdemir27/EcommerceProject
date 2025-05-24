// ProductService.Infrastructure/Persistence/Repositories/ProductRepository.cs
using Microsoft.EntityFrameworkCore; // FirstOrDefaultAsync, ToListAsync, EntityState için
using ProductService.Domain; // Product entity için
using ProductService.Domain.Repositories; // IProductRepository için

namespace ProductService.Infrastructure.Persistence.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductDbContext _context;

        public ProductRepository(ProductDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // FindAsync primary key üzerinden arama yapar.
            // Eğer Price gibi owned type'lar varsa ve bunların da yüklenmesini istiyorsak,
            // EF Core bunları genellikle otomatik olarak yükler.
            // Ancak explicit loading veya eager loading (Include) gerekebilir bazı senaryolarda.
            // Şimdilik FindAsync yeterli olacaktır.
            return await _context.Products.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            // AsNoTracking() read-only operasyonlarda performansı artırabilir.
            return await _context.Products
                // .AsNoTracking() // Eğer sadece okunacaksa eklenebilir
                .ToListAsync(cancellationToken);
        }

        // IProductRepository'ye GetByCategoryIdAsync eklersek implementasyonu:
        // public async Task<IEnumerable<Product>> GetByCategoryIdAsync(Guid categoryId, CancellationToken cancellationToken = default)
        // {
        //     return await _context.Products
        //         .Where(p => p.CategoryId == categoryId)
        //         // .AsNoTracking()
        //         .ToListAsync(cancellationToken);
        // }

        public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
        {
            await _context.Products.AddAsync(product, cancellationToken);
        }

        public void Update(Product product)
        {
            // EF Core'un değişiklik izleyicisi bu entity'yi zaten izliyorsa (örneğin context'ten okunduysa),
            // sadece özellikleri değiştirmek yeterlidir. Değilse, state'ini Modified olarak işaretlemek gerekir.
            _context.Entry(product).State = EntityState.Modified;
        }

        public async Task DeleteAsync(Product product, CancellationToken cancellationToken = default)
        {
            // Genellikle ID ile silme veya önce entity'yi bulup sonra silme yapılır.
            // Eğer entity zaten context tarafından izleniyorsa _context.Products.Remove(product) yeterlidir.
            _context.Products.Remove(product);
            await Task.CompletedTask; // Remove senkron bir operasyondur. SaveChangesAsync asıl işlemi yapar.
        }
    }
}