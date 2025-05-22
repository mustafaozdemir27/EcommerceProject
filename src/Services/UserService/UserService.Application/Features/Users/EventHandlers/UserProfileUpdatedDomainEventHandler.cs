// UserService.Application/Features/Users/EventHandlers/UserProfileUpdatedDomainEventHandler.cs
using MediatR;                           // INotificationHandler için
using Microsoft.Extensions.Logging;      // ILogger için
using UserService.Domain.Events;         // UserProfileUpdatedDomainEvent için

namespace UserService.Application.Features.Users.EventHandlers
{
    public class UserProfileUpdatedDomainEventHandler : INotificationHandler<UserProfileUpdatedDomainEvent>
    {
        private readonly ILogger<UserProfileUpdatedDomainEventHandler> _logger;

        public UserProfileUpdatedDomainEventHandler(ILogger<UserProfileUpdatedDomainEventHandler> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public Task Handle(UserProfileUpdatedDomainEvent notification, CancellationToken cancellationToken)
        {
            // Bu handler, bir UserProfileUpdatedDomainEvent yayınlandığında MediatR tarafından otomatik olarak çağrılacaktır.
            // Şimdilik, olayı sadece loglayalım.

            _logger.LogInformation(
                "Domain Event Handled: {EventName} - Kullanıcının (ID: {UserId}) profili {UpdatedAtUtc} tarihinde güncellendi. Olay Oluşma Zamanı: {OccurredOn}",
                nameof(UserProfileUpdatedDomainEvent),
                notification.UserId,
                notification.UpdatedAtUtc,
                notification.OccurredOn);

            return Task.CompletedTask;
        }
    }
}