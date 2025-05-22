// UserService.Application/Features/Users/Queries/ListUsers/ListUsersQuery.cs
using MediatR; // IRequest için
using UserService.Application.Features.Users.Dtos; // UserDto için

namespace UserService.Application.Features.Users.Queries.ListUsers
{
    public class ListUsersQuery : IRequest<IEnumerable<UserDto>>
    {
        // Bu sorgu şimdilik herhangi bir parametre almıyor.
        // İleride buraya filtreleme (örn: IsActive), sıralama veya
        // sayfalama (pagination) parametreleri eklenebilir.
        // Örneğin:
        // public bool? IsActive { get; set; }
        // public int PageNumber { get; set; } = 1;
        // public int PageSize { get; set; } = 10;
    }
}