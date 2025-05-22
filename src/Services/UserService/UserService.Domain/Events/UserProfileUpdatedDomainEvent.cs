// UserService.Domain/Events/UserProfileUpdatedDomainEvent.cs
using Common.Domain; // IDomainEvent için

namespace UserService.Domain.Events
{
    public class UserProfileUpdatedDomainEvent : IDomainEvent
    {
        public Guid UserId { get; }
        public DateTime UpdatedAtUtc { get; }

        // IDomainEvent'ten gelen özellik
        public DateTime OccurredOn { get; }

        public UserProfileUpdatedDomainEvent(Guid userId, DateTime updatedAtUtc)
        {
            UserId = userId;
            UpdatedAtUtc = updatedAtUtc; // Profilin güncellendiği zaman
            OccurredOn = DateTime.UtcNow;    // Olayın oluşturulduğu an
        }
    }
}