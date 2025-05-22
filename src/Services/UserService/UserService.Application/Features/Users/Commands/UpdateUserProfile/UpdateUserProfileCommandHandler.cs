// UserService.Application/Features/Users/Commands/UpdateUserProfile/UpdateUserProfileCommandHandler.cs
using Common.Infrastructure.Data;       // IUnitOfWork için
using MediatR;
using UserService.Application.Exceptions; // NotFoundException için
using UserService.Domain.Repositories;    // IUserRepository için
// using UserService.Domain; // User entity'si için (nameof(User) gibi ifadelerde gerekebilir)

namespace UserService.Application.Features.Users.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Unit>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateUserProfileCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Unit> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            // 1. Kullanıcıyı ID ile repository'den getir
            var userToUpdate = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

            // 2. Kullanıcı bulunamazsa NotFoundException fırlat
            if (userToUpdate == null)
            {
                throw new NotFoundException(nameof(Domain.User), request.Id); // Domain.User yerine "User" string'i de kullanılabilir
            }

            // 3. User entity'si üzerindeki UpdateProfile metodunu çağır
            // Bu metot, null gelen değerleri dikkate alarak kısmi güncellemeyi yönetir
            // ve değişiklik olduysa domain event'ini ekler.
            userToUpdate.UpdateProfile(request.FirstName, request.LastName);

            // 4. Değişiklikleri kaydet (Unit of Work)
            // UserRepository.Update(userToUpdate); metodunu çağırmamıza gerek yok
            // çünkü EF Core değişiklik izleyicisi, context'ten okunan bir entity üzerindeki
            // değişiklikleri zaten takip eder. Sadece SaveChangesAsync yeterlidir.
            // Eğer userToUpdate'i context dışında oluşturup buraya verseydik Update çağırmak gerekirdi.
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 5. İşlem başarılı olduğunda Unit.Value döndür (anlamlı bir dönüş değeri yok)
            return Unit.Value;
        }
    }
}