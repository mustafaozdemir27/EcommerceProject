// UserService.Domain/Events/UserRegisteredDomainEvent.cs
using Common.Domain; // IDomainEvent için

namespace UserService.Domain.Events
{
    public class UserRegisteredDomainEvent : IDomainEvent
    {
        public Guid UserId { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public DateTime RegisteredAtUtc { get; }

        // IDomainEvent'ten gelen özellik
        public DateTime OccurredOn { get; }

        public UserRegisteredDomainEvent(Guid userId, string email, string firstName, string lastName, DateTime registeredAtUtc)
        {
            UserId = userId;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            RegisteredAtUtc = registeredAtUtc; // Bu, kullanıcının kaydedildiği an
            OccurredOn = DateTime.UtcNow;    // Bu ise olayın oluşturulduğu an (genellikle çok yakın olurlar)
        }
    }
}