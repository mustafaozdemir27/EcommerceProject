// UserService.Application/Features/Users/Commands/UpdateUserProfile/UpdateUserProfileCommand.cs
using MediatR; // IRequest<Unit> için

namespace UserService.Application.Features.Users.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommand : IRequest<Unit> // Unit: İşlemin özel bir dönüş değeri olmadığını belirtir.
    {
        /// <summary>
        /// Güncellenecek kullanıcının ID'si.
        /// Bu genellikle API'de route parametresinden gelir.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Güncellenecek yeni Ad.
        /// Null olabilir; null ise bu alanın güncellenmeyeceği anlamına gelebilir (kısmi güncelleme).
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Güncellenecek yeni Soyad.
        /// Null olabilir; null ise bu alanın güncellenmeyeceği anlamına gelebilir.
        /// </summary>
        public string? LastName { get; set; }

        // Buraya e-posta gibi diğer güncellenebilir alanlar da eklenebilir.
        // Ancak e-posta güncellemesi, benzersizlik kontrolü ve potansiyel
        // e-posta doğrulama süreçleri nedeniyle daha dikkatli ele alınmalıdır.
    }
}