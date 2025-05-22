// UserService.Infrastructure/Persistence/UserDbContext.cs
// ... (diğer using ifadeleri) ...
using Common.Domain;
using Common.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging; // ILogger için (opsiyonel ama daha iyi loglama için)
using System.Diagnostics;
using UserService.Domain; // Debug.WriteLine için

// ...

public class UserDbContext : DbContext, IUnitOfWork
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserDbContext> _logger; // Opsiyonel: Daha yapısal loglama için

    public DbSet<User> Users { get; set; } = null!;

    // ILogger'ı enjekte etmek için constructor'ı güncelleyebiliriz (opsiyonel)
    // UserService.Infrastructure/Persistence/UserDbContext.cs
    public UserDbContext(DbContextOptions<UserDbContext> options, IMediator mediator, ILogger<UserDbContext> logger)
        : base(options)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _logger.LogWarning("UserDbContext instance CREATED with HashCode: {DbContextHashCode}", this.GetHashCode());
    }

    // ... (OnModelCreating burada) ...
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(builder =>
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
            builder.HasIndex(u => u.Username).IsUnique();
            builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(50);
            builder.Property(u => u.PasswordHash).IsRequired();
        });
    }


    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Konsola veya Debug output'a log yazdıralım
        Debug.WriteLine("UserDbContext.SaveChangesAsync - Başladı.");
        _logger.LogInformation("UserDbContext.SaveChangesAsync - Başladı."); // Opsiyonel logger

        // Değişiklik İzleyicisindeki (ChangeTracker) entity'lerin durumunu kontrol et
        var addedEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Added).ToList();
        var modifiedEntities = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified).ToList();

        Debug.WriteLine($"UserDbContext: {addedEntities.Count} adet 'Added' state'inde entity bulundu.");
        _logger.LogInformation("UserDbContext: {AddedCount} adet 'Added' state'inde entity bulundu.", addedEntities.Count);
        // Eklenen entity'lerin tiplerini de loglayabiliriz:
        foreach (var entry in addedEntities)
        {
            Debug.WriteLine($"UserDbContext: Added Entity Tipi: {entry.Entity.GetType().Name}");
            _logger.LogInformation("UserDbContext: Added Entity Tipi: {EntityType}", entry.Entity.GetType().Name);
        }

        Debug.WriteLine("UserDbContext.SaveChangesAsync - base.SaveChangesAsync çağrılıyor...");
        _logger.LogInformation("UserDbContext.SaveChangesAsync - base.SaveChangesAsync çağrılıyor...");

        int result = 0;
        try
        {
            result = await base.SaveChangesAsync(cancellationToken);
            Debug.WriteLine($"UserDbContext.SaveChangesAsync - base.SaveChangesAsync tamamlandı. Sonuç (etkilenen satır sayısı): {result}");
            _logger.LogInformation("UserDbContext.SaveChangesAsync - base.SaveChangesAsync tamamlandı. Sonuç (etkilenen satır sayısı): {ResultCount}", result);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"UserDbContext.SaveChangesAsync - base.SaveChangesAsync sırasında HATA: {ex.Message}");
            _logger.LogError(ex, "UserDbContext.SaveChangesAsync - base.SaveChangesAsync sırasında HATA oluştu.");
            throw; // Hatayı tekrar fırlat ki fark edilsin
        }

        if (result > 0) // Sadece kayıt başarılı olduysa olayları yayınla
        {
            Debug.WriteLine("UserDbContext.SaveChangesAsync - DispatchDomainEventsAsync çağrılıyor...");
            _logger.LogInformation("UserDbContext.SaveChangesAsync - DispatchDomainEventsAsync çağrılıyor...");
            await DispatchDomainEventsAsync(cancellationToken);
            Debug.WriteLine("UserDbContext.SaveChangesAsync - DispatchDomainEventsAsync tamamlandı.");
            _logger.LogInformation("UserDbContext.SaveChangesAsync - DispatchDomainEventsAsync tamamlandı.");
        }
        else
        {
            Debug.WriteLine("UserDbContext.SaveChangesAsync - Hiçbir satır etkilenmediği için domain olayları yayınlanmadı.");
            _logger.LogWarning("UserDbContext.SaveChangesAsync - Hiçbir satır etkilenmediği için domain olayları yayınlanmadı.");
        }

        Debug.WriteLine("UserDbContext.SaveChangesAsync - Tamamlandı.");
        _logger.LogInformation("UserDbContext.SaveChangesAsync - Tamamlandı.");
        return result;
    }

    // ... (DispatchDomainEventsAsync metodu burada) ...
    private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken = default)
    {
        var domainEventEntities = ChangeTracker
            .Entries<Entity<Guid>>()
            .Select(entry => entry.Entity)
            .Where(entity => entity.DomainEvents.Any())
            .ToList();

        foreach (var entity in domainEventEntities)
        {
            var events = entity.DomainEvents.ToList();
            entity.ClearDomainEvents();

            foreach (var domainEvent in events)
            {
                await _mediator.Publish(domainEvent, cancellationToken);
            }
        }
    }
}