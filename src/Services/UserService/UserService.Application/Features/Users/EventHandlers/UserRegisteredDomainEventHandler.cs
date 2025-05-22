// UserService.Application/Features/Users/EventHandlers/UserRegisteredDomainEventHandler.cs
using MediatR; // INotificationHandler için
using UserService.Domain.Events; // UserRegisteredDomainEvent için
using Microsoft.Extensions.Logging;
namespace UserService.Application.Features.Users.EventHandlers
{
    public class UserRegisteredDomainEventHandler : INotificationHandler<UserRegisteredDomainEvent>
    {
        private readonly ILogger<UserRegisteredDomainEventHandler> _logger;

        public UserRegisteredDomainEventHandler(ILogger<UserRegisteredDomainEventHandler> logger)
        {
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
        {
            // Bu handler, bir UserRegisteredDomainEvent yayınlandığında MediatR tarafından otomatik olarak çağrılacaktır.
            // Şimdilik, olayı sadece loglayalım.
            // Gerçek bir uygulamada burada e-posta gönderme, başka sistemleri güncelleme vb. işlemler yapılabilir.

            _logger.LogInformation(
                "Domain Event Handled: {EventName} - Kullanıcı ID: {UserId}, E-posta: {Email}, Kayıt Zamanı: {RegisteredAtUtc}, Olay Oluşma Zamanı: {OccurredOn}",
                nameof(UserRegisteredDomainEvent),
                notification.UserId,
                notification.Email,
                notification.RegisteredAtUtc,
                notification.OccurredOn);

            // Bu handler senkron bir işlem yaptığı için (sadece loglama),
            // Task.CompletedTask döndürmek yeterlidir.
            // Eğer asenkron bir işlem yapılsaydı (örn: await emailService.SendAsync(...)),
            // o Task döndürülürdü.
            return Task.CompletedTask;
        }
    }
}