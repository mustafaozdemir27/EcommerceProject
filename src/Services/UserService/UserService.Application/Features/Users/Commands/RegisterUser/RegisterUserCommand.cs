// UserService.Application/Features/Users/Commands/RegisterUser/RegisterUserCommand.cs
using MediatR; // IRequest için

namespace UserService.Application.Features.Users.Commands.RegisterUser
{
    public class RegisterUserCommand : IRequest<Guid> // Guid: Yeni kullanıcının ID'sini döndürecek
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty; // Şifreyi burada alıp handler'da hash'leyeceğiz
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}