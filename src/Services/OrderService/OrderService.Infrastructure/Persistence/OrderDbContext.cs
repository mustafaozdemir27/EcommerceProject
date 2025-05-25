// OrderService.Infrastructure/Persistence/OrderDbContext.cs
using Common.Domain; // Entity<Guid>, IDomainEvent için
using Common.Infrastructure.Data; // IUnitOfWork için
using MediatR; // IMediator için
using Microsoft.EntityFrameworkCore;
using OrderService.Domain; // Order, OrderItem entity'leri için
using OrderService.Domain.Enums; // OrderStatus enum için

namespace OrderService.Infrastructure.Persistence
{
    public class OrderDbContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;

        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!; // OrderItem'lar için de DbSet

        public OrderDbContext(DbContextOptions<OrderDbContext> options, IMediator mediator)
            : base(options)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Order entity'si için Fluent API konfigürasyonları
            modelBuilder.Entity<Order>(builder =>
            {
                builder.ToTable("Orders");
                builder.HasKey(o => o.Id);

                builder.Property(o => o.CustomerId).IsRequired();
                builder.Property(o => o.OrderDateUtc).IsRequired();

                // OrderStatus enum'ını string olarak saklama (veya int olarak da saklanabilir)
                builder.Property(o => o.Status)
                    .IsRequired()
                    .HasConversion(
                        s => s.ToString(), // Enum'dan string'e
                        s => (OrderStatus)Enum.Parse(typeof(OrderStatus), s) // String'den enum'a
                    )
                    .HasMaxLength(50); // Enum string değerinin max uzunluğu

                // Adres bilgilerini ayrı kolonlar olarak map'leme
                builder.Property(o => o.ShippingAddressStreet).IsRequired().HasMaxLength(200);
                builder.Property(o => o.ShippingAddressCity).IsRequired().HasMaxLength(100);
                builder.Property(o => o.ShippingAddressCountry).IsRequired().HasMaxLength(100);
                builder.Property(o => o.ShippingAddressZipCode).IsRequired().HasMaxLength(20);

                // Order ve OrderItem arasındaki ilişki (One-to-Many)
                // OrderItem'ın OrderId foreign key'i EF Core tarafından konvansiyonel olarak bulunur,
                // ancak açıkça belirtmek daha iyi olabilir.
                // OrderItems koleksiyonu zaten Order içinde tanımlı.
                // OrderItem'da OrderId alanı zaten var. EF Core bu ilişkiyi genellikle otomatik kurar.
                // Gerekirse: builder.HasMany(o => o.OrderItems).WithOne().HasForeignKey(oi => oi.OrderId);
                // Ancak OrderItem'ın Order'a bir navigation property'si yoksa WithOne() parametresiz olur.
                // Bizim OrderItem'ımızda OrderId var ama Order navigation property'si yok, bu yüzden bu şekilde yeterli.
            });

            // OrderItem entity'si için Fluent API konfigürasyonları
            modelBuilder.Entity<OrderItem>(builder =>
            {
                builder.ToTable("OrderItems");
                builder.HasKey(oi => oi.Id);

                builder.Property(oi => oi.ProductId).IsRequired();
                builder.Property(oi => oi.ProductName).IsRequired().HasMaxLength(200); // Ürün adı için makul bir uzunluk

                builder.Property(oi => oi.UnitPrice)
                    .HasColumnType("decimal(18,2)") // Veritabanı tipi ve hassasiyeti
                    .IsRequired();

                builder.Property(oi => oi.Currency)
                    .IsRequired()
                    .HasMaxLength(3);

                builder.Property(oi => oi.Quantity).IsRequired();

                // OrderId foreign key'i (Order tablosuna işaret eder)
                // İlişki Order entity'sinde HasMany ile tanımlanabilir veya EF Core konvansiyonuyla bulunur.
                // builder.HasOne<Order>().WithMany(o => o.OrderItems).HasForeignKey(oi => oi.OrderId); 
                // Bu tür bir yapılandırma, Order'da OrderItems için bir navigation property ve
                // OrderItem'da Order'a bir navigation property (ve OrderId FK) olduğunda daha yaygındır.
                // Bizim OrderItem'ımızda OrderId FK var.
            });
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
                .Entries<Entity<Guid>>() // Order ve OrderItem, Entity<Guid>'den miras alıyor (OrderItem'ı da Entity<Guid> yaptık)
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