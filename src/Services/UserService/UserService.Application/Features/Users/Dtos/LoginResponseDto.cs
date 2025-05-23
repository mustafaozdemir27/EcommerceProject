// UserService.Application/Features/Users/Dtos/LoginResponseDto.cs
namespace UserService.Application.Features.Users.Dtos
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        // İsteğe bağlı olarak roller veya diğer kullanıcı bilgileri de eklenebilir.
    }
}