// Common.Infrastructure/Data/IUnitOfWork.cs
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Infrastructure.Data
{
    /// <summary>
    /// Unit of Work desenini temsil eden arayüz.
    /// Veritabanı işlemlerinin bir bütün olarak yönetilmesini sağlar.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Bu Unit of Work kapsamında yapılan tüm değişiklikleri asenkron olarak veritabanına kaydeder.
        /// </summary>
        /// <param name="cancellationToken">İşlemin iptal edilebilmesi için kullanılan token.</param>
        /// <returns>Veritabanında etkilenen satır sayısını içeren bir Task.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        // Daha karmaşık senaryolarda veya farklı bir Repository yönetim yaklaşımında,
        // IUnitOfWork üzerinden Repository'lere erişim sağlamak için metotlar eklenebilir. Örneğin:
        //
        // IRepository<TEntity, TId> GetRepository<TEntity, TId>()
        //     where TEntity : Entity<TId>, IAggregateRoot
        //     where TId : IEquatable<TId>;
        //
        // Veya her mikroservisin kendi özel repository arayüzleri için:
        //
        // IUserRepository Users { get; } // UserService'de
        // IProductRepository Products { get; } // ProductService'de
        //
        // Ancak şimdilik sadece SaveChangesAsync yeterli olacaktır.
        // Repository'leri doğrudan dependency injection ile alacağız.
    }
}