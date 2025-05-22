// UserService.Application/Features/Users/Dtos/UserDto.cs
namespace UserService.Application.Features.Users.Dtos
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public bool IsActive { get; set; }

        // İhtiyaç duyulursa buraya kullanıcının rolleri,
        // son giriş tarihi gibi ek bilgiler de eklenebilir.
    }
}