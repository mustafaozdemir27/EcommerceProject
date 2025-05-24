// ProductService.Infrastructure/Persistence/ProductDbContext.cs
using Common.Domain; // Entity<Guid>, IDomainEvent için
using Common.Infrastructure.Data; // IUnitOfWork için
using MediatR; // IMediator için
using Microsoft.EntityFrameworkCore;
using ProductService.Domain; // Product entity ve Price VO için

namespace ProductService.Infrastructure.Persistence
{
    public class ProductDbContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;

        // DbSet'lerimizi tanımlıyoruz
        public DbSet<Product> Products { get; set; } = null!;
        // İleride Category gibi başka Aggregate Root'lar eklenirse, onlar için de DbSet'ler eklenecek.
        // public DbSet<Category> Categories { get; set; } = null!;

        public ProductDbContext(DbContextOptions<ProductDbContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Product entity'si için Fluent API konfigürasyonları
            modelBuilder.Entity<Product>(builder =>
            {
                builder.ToTable("Products"); // Veritabanındaki tablo adı
                builder.HasKey(p => p.Id);   // Primary Key

                builder.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                builder.Property(p => p.Description)
                    .HasMaxLength(1000);

                // Price Value Object'ini OwnsOne ile owned type olarak yapılandırma
                // Bu, Price özelliklerinin Products tablosunda ayrı kolonlar olarak (örn: CurrentPrice_Amount, CurrentPrice_Currency)
                // veya JSON olarak (EF Core 7+ ile) saklanmasını sağlar.
                // Varsayılan olarak ayrı kolonlar oluşturur.
                builder.OwnsOne(p => p.CurrentPrice, priceBuilder =>
                {
                    priceBuilder.Property(pp => pp.Amount)
                        .HasColumnName("PriceAmount") // Veritabanındaki kolon adı
                        .HasColumnType("decimal(18,2)") // Veritabanı tipi ve hassasiyeti
                        .IsRequired();

                    priceBuilder.Property(pp => pp.Currency)
                        .HasColumnName("PriceCurrency")
                        .IsRequired()
                        .HasMaxLength(3); // "TRY", "USD" gibi 3 karakterlik kodlar için
                });

                builder.Property(p => p.StockQuantity)
                    .IsRequired();

                builder.Property(p => p.CategoryId)
                    .IsRequired();

                // Zaman damgaları
                builder.Property(p => p.CreatedAtUtc).IsRequired();
                builder.Property(p => p.UpdatedAtUtc);

                // Index'ler (gerekirse)
                // builder.HasIndex(p => p.Name);
                // builder.HasIndex(p => p.CategoryId);
            });

            // Eğer Category entity'si de bu context'te yönetilecekse, onun için de konfigürasyonlar buraya gelir.
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var result = await base.SaveChangesAsync(cancellationToken);
            await DispatchDomainEventsAsync(cancellationToken); // Domain olaylarını yayınla
            return result;
        }

        private async Task DispatchDomainEventsAsync(CancellationToken cancellationToken = default)
        {
            var domainEventEntities = ChangeTracker
                .Entries<Entity<Guid>>() // Product, Entity<Guid>'den miras alıyor
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
}