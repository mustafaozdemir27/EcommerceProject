// Common.Infrastructure/Data/IRepository.cs
using Common.Domain; // Entity<TId> ve IAggregateRoot için
using System.Linq.Expressions;

namespace Common.Infrastructure.Data
{
    /// <summary>
    /// Bir Aggregate Root için genel Repository arayüzü.
    /// </summary>
    /// <typeparam name="TEntity">Yönetilecek Entity tipi (bir Aggregate Root olmalı).</typeparam>
    /// <typeparam name="TId">Entity'nin ID tipi.</typeparam>
    public interface IRepository<TEntity, TId>
        where TEntity : Entity<TId>, IAggregateRoot // TEntity'nin hem Entity<TId> hem de IAggregateRoot olmasını zorunlu kılar
        where TId : IEquatable<TId>
    {
        /// <summary>
        /// Verilen ID'ye sahip Entity'yi getirir. Bulunamazsa null döner.
        /// </summary>
        Task<TEntity?> GetByIdAsync(TId id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tüm Entity'leri getirir.
        /// </summary>
        Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Belirli bir koşula uyan Entity'leri getirir.
        /// </summary>
        Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Yeni bir Entity ekler.
        /// </summary>
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Mevcut bir Entity'yi günceller.
        /// (EF Core gibi ORM'lerde bu genellikle context üzerinden izlendiği için async olmayabilir.)
        /// </summary>
        void Update(TEntity entity);

        /// <summary>
        /// Bir Entity'yi siler.
        /// </summary>
        void Delete(TEntity entity);

        // İsteğe bağlı olarak eklenebilecek diğer metotlar:
        // Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default);
        // Task<bool> ExistsAsync(TId id, CancellationToken cancellationToken = default);
    }
}