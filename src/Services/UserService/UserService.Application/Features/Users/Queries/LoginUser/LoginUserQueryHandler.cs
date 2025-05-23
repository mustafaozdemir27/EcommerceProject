// UserService.Application/Features/Users/Queries/LoginUser/LoginUserQueryHandler.cs
using MediatR;
using UserService.Application.Contracts.Security; // IPasswordHasherService, IJwtTokenGenerator için
using UserService.Application.Exceptions;         // NotFoundException (veya yeni bir AuthenticationFailedException)
using UserService.Application.Features.Users.Dtos; // LoginResponseDto için
using UserService.Domain;                         // User entity için
using UserService.Domain.Repositories;            // IUserRepository için

namespace UserService.Application.Features.Users.Queries.LoginUser
{
    public class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, LoginResponseDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public LoginUserQueryHandler(
            IUserRepository userRepository,
            IPasswordHasherService passwordHasher,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _jwtTokenGenerator = jwtTokenGenerator ?? throw new ArgumentNullException(nameof(jwtTokenGenerator));
        }

        public async Task<LoginResponseDto> Handle(LoginUserQuery request, CancellationToken cancellationToken)
        {
            // 1. Kullanıcıyı e-posta veya kullanıcı adına göre bulmaya çalış
            // E-posta olup olmadığını anlamak için basit bir kontrol yapılabilir
            User? user = null;
            if (request.EmailOrUsername.Contains("@")) // Basit bir e-posta formatı kontrolü
            {
                user = await _userRepository.GetByEmailAsync(request.EmailOrUsername.ToLowerInvariant(), cancellationToken);
            }
            else
            {
                user = await _userRepository.GetByUsernameAsync(request.EmailOrUsername, cancellationToken);
            }

            // 2. Kullanıcı bulunamazsa veya aktif değilse hata fırlat
            if (user == null || !user.IsActive)
            {
                // Daha spesifik bir exception fırlatmak daha iyi olur.
                // Örneğin: AuthenticationFailedException veya InvalidCredentialsException
                // Şimdilik NotFoundException'ı veya genel bir exception'ı farklı bir mesajla kullanabiliriz.
                throw new NotFoundException("Kullanıcı bulunamadı veya hesap aktif değil.");
                // veya throw new ApplicationException("Geçersiz kullanıcı adı/e-posta veya şifre.");
            }

            // 3. Şifreyi doğrula
            bool isPasswordValid = _passwordHasher.VerifyPassword(user.PasswordHash, request.Password);
            if (!isPasswordValid)
            {
                // throw new ApplicationException("Geçersiz kullanıcı adı/e-posta veya şifre.");
                // Güvenlik açısından, kullanıcı adı mı şifre mi yanlış belirtilmemeli.
                throw new NotFoundException("Kullanıcı bulunamadı veya hesap aktif değil."); // Aynı mesajı kullanabiliriz
            }

            // JWT Token oluştur (GeneratedToken nesnesi dönecek)
            var generatedTokenInfo = _jwtTokenGenerator.GenerateToken(user, null); // Roller şimdilik null

            // LoginResponseDto'yu oluştur ve döndür
            return new LoginResponseDto
            {
                UserId = user.Id,
                Username = user.Username,
                Token = generatedTokenInfo.Token,       // Token'ı GeneratedToken'dan al
                ExpiresAt = generatedTokenInfo.ExpiresAt  // ExpiresAt'ı GeneratedToken'dan al
            };
        }
    }
}