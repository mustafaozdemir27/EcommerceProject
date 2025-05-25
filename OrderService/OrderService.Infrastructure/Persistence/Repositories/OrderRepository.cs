// OrderService.Infrastructure/Persistence/Repositories/OrderRepository.cs
using Microsoft.EntityFrameworkCore; // FirstOrDefaultAsync, ToListAsync, Include, EntityState için
using OrderService.Domain; // Order entity için
using OrderService.Domain.Repositories; // IOrderRepository için

namespace OrderService.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _context;

        public OrderRepository(OrderDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // Siparişi getirirken, ilişkili OrderItem'ları da yüklemek (eager loading) genellikle istenir.
            return await _context.Orders
                .Include(o => o.OrderItems) // OrderItems koleksiyonunu da yükle
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }

        public async Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return await _context.Orders
                .Where(o => o.CustomerId == customerId)
                .Include(o => o.OrderItems) // Her siparişin kalemlerini de yükle
                .OrderByDescending(o => o.OrderDateUtc) // En yeni siparişler üste gelecek şekilde sırala (opsiyonel)
                                                        // .AsNoTracking() // Eğer sadece okunacaksa ve değişiklik izlenmeyecekse eklenebilir
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            await _context.Orders.AddAsync(order, cancellationToken);
            // Not: Order aggregate root olduğu için, ona bağlı OrderItem'lar da
            // EF Core tarafından otomatik olarak takip edilir ve AddAsync ile eklendiğinde
            // Order ile birlikte kaydedilir (eğer Order'ın OrderItems koleksiyonuna eklendilerse).
        }

        public void Update(Order order)
        {
            // EF Core'un değişiklik izleyicisi, context'ten okunan bir entity üzerindeki
            // değişiklikleri zaten takip eder. State'ini Modified olarak işaretlemek,
            // entity'nin context dışında değiştirilip sonra update edilmek istendiği durumlarda daha yaygındır.
            _context.Entry(order).State = EntityState.Modified;

            // Eğer OrderItems koleksiyonunda da değişiklikler (ekleme, silme, güncelleme) varsa,
            // EF Core bunları da (ilişki doğru yapılandırıldıysa) takip etmelidir.
            // Kompleks senaryolarda OrderItem'ların state'lerini ayrıca yönetmek gerekebilir.
        }

        // IOrderRepository'ye DeleteAsync eklersek implementasyonu:
        // public async Task DeleteAsync(Order order, CancellationToken cancellationToken = default)
        // {
        //     _context.Orders.Remove(order); // Order'ı silmek, ilişkili OrderItem'ları da (cascade delete ayarına bağlı olarak) silebilir.
        //     await Task.CompletedTask;
        // }
    }
}