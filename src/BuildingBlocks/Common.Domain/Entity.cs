// Common.Domain/Entity.cs
namespace Common.Domain
{
    public abstract class Entity<TId> where TId : IEquatable<TId>
    {
        public TId Id { get; protected set; }

        private readonly List<IDomainEvent> _domainEvents = new List<IDomainEvent>();

        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }

        protected Entity(TId id)
        {
            if (object.Equals(id, default(TId)))
            {
                // ID'nin varsayılan bir değer olmamasını sağlamak için bir istisna fırlatılabilir
                // veya bir Guid atanabilir. Şimdilik basit tutalım.
                // throw new ArgumentException("The ID cannot be the default value.", nameof(id));
            }
            Id = id;
        }

        // EF Core gibi ORM'ler için parametresiz constructor gerekebilir.
        protected Entity()
        {
            // Bu constructor, EF Core'un entity'leri materyalize etmesi için gereklidir.
            // ID'nin varsayılan bir değer olmaması için burada da kontrol yapılabilir.
            // Ancak, bu durumda ID'nin null veya varsayılan değer olabileceğini kabul ediyoruz.
            Id = default!;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Entity<TId> other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (GetType() != other.GetType())
                return false;

            // Eğer Id null veya varsayılan değer ise, referans eşitliğine geri dönebiliriz
            // veya bunları eşit kabul etmeyebiliriz. Transient (kalıcı olmayan) varlıklar için bu durum önemlidir.
            // Şimdilik, her iki Id'nin de geçerli olduğunu varsayalım.
            if (Id == null || Id.Equals(default(TId)) || other.Id == null || other.Id.Equals(default(TId)))
            {
                return false; // Veya farklı bir strateji izlenebilir
            }

            return Id.Equals(other.Id);
        }

        public static bool operator ==(Entity<TId>? a, Entity<TId>? b)
        {
            if (a is null && b is null)
                return true;

            if (a is null || b is null)
                return false;

            return a.Equals(b);
        }

        public static bool operator !=(Entity<TId>? a, Entity<TId>? b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            // Id null değilse ve bir değere sahipse GetHashCode'u kullanın.
            // GetType() dahil etmek, farklı entity tiplerinin (aynı Id'ye sahip olsalar bile)
            // farklı hash kodlarına sahip olmasını sağlar.
            if (Id == null || Id.Equals(default(TId)))
            {
                return base.GetHashCode(); // Veya başka bir strateji
            }
            return (GetType().ToString() + Id).GetHashCode();
        }
    }
}