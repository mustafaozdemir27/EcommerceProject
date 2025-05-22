// UserService.Application/Mappings/UserProfile.cs
using AutoMapper;
using UserService.Application.Features.Users.Dtos; // UserDto için
using UserService.Domain; // User entity için

namespace UserService.Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            // User entity'sinden UserDto'ya mapping tanımı:
            CreateMap<User, UserDto>();

            // İhtiyaç duyulursa UserDto'dan User entity'sine ters yönde bir mapping de tanımlanabilir:
            // CreateMap<UserDto, User>();
            // Ancak bu genellikle güncellemelerde dikkatli kullanılmalıdır,
            // çünkü DTO'dan gelen her alanın entity'ye doğrudan yazılması istenmeyebilir.

            // RegisterUserCommand'dan User entity'sine bir mapping de tanımlanabilir,
            // ancak şu anki RegisterUserCommandHandler'ımızda User entity'sini manuel olarak oluşturuyoruz.
            // CreateMap<Features.Users.Commands.RegisterUser.RegisterUserCommand, User>();
        }
    }
}