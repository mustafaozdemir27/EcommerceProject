// UserService.Application/Features/Users/Commands/RegisterUser/RegisterUserCommandHandler.cs
using Common.Infrastructure.Data; // IUnitOfWork için
using MediatR;
using UserService.Application.Exceptions; // DuplicateValueException için
using UserService.Domain; // User entity için
using UserService.Domain.Repositories; // IUserRepository için
// Şifreleme için using System.Security.Cryptography; ve System.Text; eklenebilir

namespace UserService.Application.Features.Users.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        // private readonly IPasswordHasher _passwordHasher; // İleride eklenecek

        public RegisterUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork /*, IPasswordHasher passwordHasher */)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            // _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        }

        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // 1. Doğrulama (FluentValidation ile daha sonra detaylandırılacak)
            // Şimdilik temel null/empty kontrolleri User entity constructor'ında yapılıyor.
            // Ek olarak burada da yapılabilir. Örneğin, şifre karmaşıklığı.

            // 2. Kullanıcı adı ve e-posta benzersizlik kontrolü
            if (await _userRepository.GetByUsernameAsync(request.Username, cancellationToken) != null)
            {
                // Daha önce: throw new Exception($"Username '{request.Username}' already exists.");
                // Şimdi:
                throw new DuplicateValueException(nameof(User), nameof(request.Username), request.Username);
            }

            if (await _userRepository.GetByEmailAsync(request.Email.ToLowerInvariant(), cancellationToken) != null)
            {
                // Daha önce: throw new Exception($"Email '{request.Email}' already exists.");
                // Şimdi:
                throw new DuplicateValueException(nameof(User), nameof(request.Email), request.Email);
            }

            // 3. Şifre Hash'leme (Basit bir örnek - üretim için ASLA bunu kullanmayın!)
            // Gerçek bir uygulamada ASP.NET Core Identity'nin PasswordHasher'ı veya PBKDF2, Argon2, bcrypt gibi
            // güçlü bir kütüphane kullanılmalıdır. Şimdilik konsepti göstermek için çok basit bir hash yapalım.
            // İleride bu bir IPasswordHasherService ile soyutlanacaktır.
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password); // Örnek: BCrypt.Net NuGet paketi ile
            // Eğer BCrypt.Net kullanacaksanız, UserService.Application projesine bu NuGet paketini eklemeniz gerekir.
            // VEYA ŞİMDİLİK:
            // string hashedPassword = Convert.ToBase64String(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(request.Password)));


            // 4. User entity'sini oluşturma
            var newUser = new User(
                Guid.NewGuid(), // Yeni kullanıcı için ID oluştur
                request.Username,
                request.Email,
                hashedPassword,
                request.FirstName,
                request.LastName
            );
            // User constructor'ı içinde UserRegisteredDomainEvent zaten AddDomainEvent ile ekleniyor.

            // 5. Kullanıcıyı repository'ye ekleme
            await _userRepository.AddAsync(newUser, cancellationToken);

            // 6. Değişiklikleri kaydetme (Unit of Work)
            // Bu işlem, kullanıcıyı veritabanına kaydeder.
            // Domain olaylarının yayınlanması genellikle bu adımdan sonra tetiklenir (MediatR pipeline'ı veya DbContext üzerinden).
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // 7. Yeni kullanıcının ID'sini döndürme
            return newUser.Id;
        }
    }
}