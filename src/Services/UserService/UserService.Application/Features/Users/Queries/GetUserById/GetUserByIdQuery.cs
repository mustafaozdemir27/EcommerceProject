// UserService.Application/Features/Users/Queries/GetUserById/GetUserByIdQuery.cs
using MediatR;
using UserService.Application.Features.Users.Dtos; // UserDto için

namespace UserService.Application.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQuery : IRequest<UserDto?> // UserDto? : Kullanıcı bulunamazsa null dönebilir
    {
        public Guid Id { get; } // ID'nin dışarıdan değiştirilmemesi için sadece get

        public GetUserByIdQuery(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("User ID cannot be empty.", nameof(id));
            }
            Id = id;
        }
    }
}