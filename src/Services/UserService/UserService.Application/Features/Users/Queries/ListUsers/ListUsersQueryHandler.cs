// UserService.Application/Features/Users/Queries/ListUsers/ListUsersQueryHandler.cs
using AutoMapper; // IMapper için
using MediatR;
using UserService.Application.Features.Users.Dtos; // UserDto için
using UserService.Domain.Repositories; // IUserRepository için

namespace UserService.Application.Features.Users.Queries.ListUsers
{
    public class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, IEnumerable<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public ListUsersQueryHandler(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository ?? throw new System.ArgumentNullException(nameof(userRepository));
            _mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<UserDto>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
        {
            // 1. Repository üzerinden tüm kullanıcıları getir
            var users = await _userRepository.GetAllAsync(cancellationToken);

            // 2. User entity listesini UserDto listesine maple
            // _mapper.Map metodu, koleksiyonlar için de çalışır.
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

            // 3. DTO listesini döndür
            return userDtos;
        }
    }
}