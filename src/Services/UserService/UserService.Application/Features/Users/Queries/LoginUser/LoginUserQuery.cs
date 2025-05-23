// UserService.Application/Features/Users/Queries/LoginUser/LoginUserQuery.cs
using MediatR;
using UserService.Application.Features.Users.Dtos;

namespace UserService.Application.Features.Users.Queries.LoginUser
{
    public class LoginUserQuery : IRequest<LoginResponseDto>
    {
        public string EmailOrUsername { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}