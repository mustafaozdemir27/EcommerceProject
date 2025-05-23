// UserService.Application/Features/Users/Commands/RegisterUser/RegisterUserCommandHandler.cs
using Common.Infrastructure.Data;
using MediatR;
using UserService.Application.Contracts.Security; // IPasswordHasherService için
using UserService.Application.Exceptions;
using UserService.Domain;
using UserService.Domain.Repositories;

namespace UserService.Application.Features.Users.Commands.RegisterUser
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Guid>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasherService _passwordHasher; // YENİ: Servisi enjekte et

        public RegisterUserCommandHandler(
            IUserRepository userRepository,
            IUnitOfWork unitOfWork,
            IPasswordHasherService passwordHasher) // YENİ: Constructor parametresi
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher)); // YENİ: Ata
        }

        public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            // ... (Benzersizlik kontrolleri burada) ...
            if (await _userRepository.GetByUsernameAsync(request.Username, cancellationToken) != null)
            {
                throw new DuplicateValueException(nameof(User), nameof(request.Username), request.Username);
            }
            var emailToCheck = request.Email.ToLowerInvariant();
            if (await _userRepository.GetByEmailAsync(emailToCheck, cancellationToken) != null)
            {
                throw new DuplicateValueException(nameof(User), nameof(request.Email), request.Email);
            }

            // Şifre Hash'leme (YENİ YÖNTEM)
            string hashedPassword = _passwordHasher.HashPassword(request.Password);
            // ESKİ YÖNTEM: string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // ... (User entity'si oluşturma, repository'ye ekleme, SaveChangesAsync) ...
            var newUser = new User(
                Guid.NewGuid(),
                request.Username,
                request.Email,
                hashedPassword,
                request.FirstName,
                request.LastName
            );
            await _userRepository.AddAsync(newUser, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return newUser.Id;
        }
    }
}