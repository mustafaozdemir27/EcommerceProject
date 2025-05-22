// Common.Domain/IDomainEvent.cs
using MediatR; // MediatR paketini ekledikten sonra bu using ifadesi geçerli olacaktır.

namespace Common.Domain
{
    /// <summary>
    /// Alan olaylarını temsil eden temel arayüz.
    /// MediatR.INotification'dan kalıtım alarak MediatR pipeline'ında işlenebilirler.
    /// </summary>
    public interface IDomainEvent : INotification
    {
        /// <summary>
        /// Olayın meydana geldiği zaman.
        /// </summary>
        DateTime OccurredOn { get; }

        // İsteğe bağlı olarak her olaya benzersiz bir kimlik de eklenebilir:
        // Guid EventId { get; }
    }
}