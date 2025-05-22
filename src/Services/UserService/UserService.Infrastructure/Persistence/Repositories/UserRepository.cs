// UserService.Infrastructure/Persistence/Repositories/UserRepository.cs
using Microsoft.EntityFrameworkCore; // FirstOrDefaultAsync, EntityState için
using UserService.Domain; // User entity için
using UserService.Domain.Repositories; // IUserRepository için

namespace UserService.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _context;

        public UserRepository(UserDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // FindAsync metodu primary key üzerinden arama yapar.
            // Eğer entity context tarafından zaten izleniyorsa, veritabanına gitmeden onu döndürür.
            return await _context.Users.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
        }

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            // User entity'sinde e-postayı küçük harf olarak sakladığımız için,
            // arama yaparken de parametreyi küçük harfe çeviriyoruz.
            var emailLower = email.ToLowerInvariant();
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == emailLower, cancellationToken);
        }

        // UserService.Infrastructure/Persistence/Repositories/UserRepository.cs
        public async Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(user, cancellationToken);
            // Bu satırın çıktısı çok önemli:
            System.Diagnostics.Debug.WriteLine($"UserRepository.AddAsync - User ID: {user.Id}, Context HashCode: {_context.GetHashCode()}, Entity State: {_context.Entry(user).State}");
            // Eğer UserDbContext'e ILogger enjekte ettiyseniz, onunla da loglayabilirsiniz:
            // _logger.LogWarning("UserRepository.AddAsync - User ID: {UserId}, Context HashCode: {ContextHashCode}, Entity State: {EntityState}", user.Id, _context.GetHashCode(), _context.Entry(user).State);
        }

        public void Update(User user)
        {
            // EF Core'un değişiklik izleyicisi (change tracker) güncellemeleri yönetir.
            // Eğer entity context'ten okunmuşsa, EF Core zaten değişiklikleri izler
            // ve SaveChangesAsync çağrıldığında UPDATE sorgusu gönderir.
            // Eğer entity 'detached' (bağlamdan kopuk) ise,
            // onu context'e 'Modified' state'inde eklemek gerekir.
            // En güvenli yollardan biri, entity'nin state'ini açıkça Modified olarak işaretlemektir.
            _context.Entry(user).State = EntityState.Modified;
        }

        public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            // Tüm kullanıcıları Users DbSet'inden çek ve bir liste olarak döndür.
            // AsNoTracking() metodu, verilerin sadece okunacağı ve EF Core tarafından
            // değişiklikler için izlenmeyeceği durumlarda performansı artırabilir.
            // Ancak, eğer bu kullanıcılar üzerinde hemen ardından bir güncelleme yapmayacaksak
            // ve sadece DTO'ya maplenecekse AsNoTracking() iyi bir optimizasyondur.
            // Şimdilik basit tutalım, ileride optimizasyon olarak eklenebilir.
            return await _context.Users.ToListAsync(cancellationToken);
        }

        // IUserRepository'de tanımlanmışsa diğer metotların implementasyonları da buraya gelir:
        // public async Task DeleteAsync(User user, CancellationToken cancellationToken = default)
        // {
        //     _context.Users.Remove(user);
        //     await Task.CompletedTask; // Remove senkron bir operasyondur, AddAsync gibi değil.
        // }
    }
}