// UserService.Domain/Repositories/IUserRepository.cs
namespace UserService.Domain.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Verilen ID'ye sahip kullanıcıyı getirir.
        /// </summary>
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verilen kullanıcı adına sahip kullanıcıyı getirir.
        /// </summary>
        Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verilen e-posta adresine sahip kullanıcıyı getirir.
        /// </summary>
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Yeni bir kullanıcı ekler.
        /// </summary>
        Task AddAsync(User user, CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut bir kullanıcıyı günceller.
        /// (EF Core gibi ORM'lerde bu genellikle context üzerinden izlendiği için async olmayabilir.)
        /// </summary>
        void Update(User user);

        /// <summary>
        /// Tüm kullanıcıları getirir.
        /// </summary>
        Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellationToken = default);

        // İleride eklenebilecek diğer metotlar:
        // Task DeleteAsync(User user, CancellationToken cancellationToken = default);
        // Task<bool> ExistsWithUsernameAsync(string username, CancellationToken cancellationToken = default);
        // Task<bool> ExistsWithEmailAsync(string email, CancellationToken cancellationToken = default);
    }
}